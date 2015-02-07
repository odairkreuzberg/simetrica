using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using RP.Sistema.Model.Entities;
using RP.Util;
using System.Web.Script.Serialization;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using System.IO;
using ImageProcessor;
using RP.DataAccess.Interfaces;

namespace RP.Sistema.BLL
{
    public class UsuarioBLL : DataAccess.Repository<Usuario>
    {
        private void EnviarEmailComNovaSenha(string email, string novaSenha)
        {
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

            message.To.Add(email);
            message.Subject = "Senha para acesso ao sistema";
            message.Body = (string.Format("Olá, sua senha de acesso ao sistema é: <strong>{0}</strong>. Para sua maior segurança, após entrar no sistema altere sua senha.", novaSenha));
            message.IsBodyHtml = true;

            message.Priority = System.Net.Mail.MailPriority.Normal;
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            smtp.Send(message);
        }

        public UsuarioBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        #region Insert
        protected override void BeforeInsert(Usuario bean)
        {
            if (this.Exist(u => u.dsLogin.ToLower() == bean.dsLogin))
            {
                throw new Exception("O login informado já está sendo usado por outro usuário.");
            }
        }

        public override void Insert(Usuario bean)
        {
            string senha = RP.Util.Class.Util.randomString(5, true, false);
            bean.fzUsuario = RP.Util.Fonetiza.Fonetizar(bean.nmUsuario);
            bean.dsSenha = RP.Util.Class.Util.getHash(senha);

            base.Insert(bean);
            
            EnviarEmailComNovaSenha(bean.dsEmail, senha);
        }

        protected override void AfterInsert(Usuario bean)
        {
            AddPerfilPadrao(bean);
            UpdatePerfis(bean);
        }

        private void AddPerfilPadrao(Usuario bean)
        {
            var perfil = new PerfilUsuario
            {
                idPerfil = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["idPerfilPadrao"]),
                idUsuario = bean.idUsuario
            };

            var _bll = new PerfilUsuarioBLL(db,_idUsuario);
            _bll.Insert(perfil);
        }
        #endregion

        #region Update
        protected override void BeforeUpdate(Usuario bean)
        {
            if (this.Exist(u => u.dsLogin.ToLower() == bean.dsLogin && u.idUsuario != bean.idUsuario))
            {
                throw new Exception("O login informado já está sendo usado por outro usuário.");
            }
            bean.fzUsuario = RP.Util.Fonetiza.Fonetizar(bean.nmUsuario);
        }

        public void UpdateLoginCount(Usuario bean)
        {
            ((Model.Context)db).Entry(bean).Property(e => e.nrFalhalogin).IsModified = true;
        }

        public void ResetarSenha(Usuario bean)
        {
            string senha = RP.Util.Class.Util.randomString(5, true, false);
            bean.dsSenha = RP.Util.Class.Util.getHash(senha);
            bean.dtValidade = DateTime.Now.AddDays(0);

            this.Update(bean);
            this.EnviarEmailComNovaSenha(bean.dsEmail, senha);
        }
        #endregion

        #region Delete
        private new void BeforeDelete(Usuario bean)
        {
            PerfilUsuarioBLL perfilUsuarioBLL = new PerfilUsuarioBLL(db, _idUsuario);
            
            foreach (var perfil in bean.Perfis) 
            {
                perfilUsuarioBLL.Delete(u => u.idPerfil == perfil.idPerfil && u.idUsuario == perfil.idUsuario);
            }
        }

        public void Delete(int id)
        {
            Usuario usuario = this.FindSingle(u => u.idUsuario == id, i => i.Perfis);
            
            if (usuario != null)
            {
                BeforeDelete(usuario);
                base.Delete(usuario);
            }
        }
        #endregion

        public void UpdatePerfis(Usuario bean)
        {
            // verifica se o perfil padrao esta na lista de perfis
            //AddPerfilPadrao(bean);
            int idPerfilPadrao = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["idPerfilPadrao"]);

            // instancia bll
            PerfilUsuarioBLL perfilUsuarioBLL = new PerfilUsuarioBLL(db, _idUsuario);

            // busca todos perfis do usuario (banco)
            List<PerfilUsuario> perfisDB = perfilUsuarioBLL.Find(u => u.idUsuario == bean.idUsuario, i => i.Perfil).ToList();

            // obtem todos perfis do usuario (view)
            List<PerfilUsuario> perfisView = bean.Perfis.ToList();

            // percorre os perfis do banco
            foreach (PerfilUsuario item in perfisDB)
            {
                if (item.idPerfil != idPerfilPadrao)
                {
                    // se o perfil percorrido nao estiver na view
                    if (!perfisView.Any(u => u.idPerfil == item.idPerfil))
                    {
                        // remove o perfil do usuario
                        perfilUsuarioBLL.Delete(u => u.idUsuario == bean.idUsuario && u.idPerfil == item.idPerfil);
                    }
                }
            }

            // percorre os perfis da view
            foreach (PerfilUsuario item in perfisView)
            {
                if (item.idPerfil != idPerfilPadrao)
                {
                    // se o perfil percorrido nao estiver no banco
                    if (!perfisDB.Any(u => u.idPerfil == item.idPerfil))
                    {
                        // insere usuario no Modulo
                        perfilUsuarioBLL.Insert(new PerfilUsuario { idPerfil = item.idPerfil, idUsuario = bean.idUsuario });
                    }
                }
            }
        }

        #region Search
        public RP.DataAccess.PaginatedList<Usuario> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Usuario> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Usuario>(query.OrderBy(o => o.idUsuario), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public RP.DataAccess.PaginatedList<Usuario> Search(string nome, string email, string login, int? page, int? pagesize)
        {
            IQueryable<Usuario> query = preSearch(nome, email, login);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Usuario>(query.OrderBy(o => o.idUsuario), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Usuario> Search(string filter)
        {
            return preSearch(filter).ToList();
        }

        private IQueryable<Usuario> preSearch(string filter)
        {

            IQueryable<Usuario> query = this.Find();

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    string fz = RP.Util.Fonetiza.Fonetizar(word);

                    query = query.Where(p => p.fzUsuario.Contains(fz) || p.nmUsuario.ToLower().Contains(temp.ToLower()));
                }
            }
            return query.AsNoTracking();
        }

        private IQueryable<Usuario> preSearch(string nome, string email, string login)
        {

            IQueryable<Usuario> query = this.Find();

            if (!string.IsNullOrEmpty(nome))
            {
                string fz = RP.Util.Fonetiza.Fonetizar(nome);

                query = query.Where(p => p.fzUsuario.Contains(fz) || p.nmUsuario.ToLower().Contains(nome.ToLower()));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.dsEmail.ToLower().Contains(email.ToLower()));
            }

            if (!string.IsNullOrEmpty(login))
            {
                query = query.Where(p => p.dsLogin.ToLower().Contains(login.ToLower()));
            }

            return query.AsNoTracking();
        }
        #endregion

        #region Preferencias
        public Usuario.Preferencias GetPreferencias(int idUsuario) 
        {
            Usuario usuario = this.FindSingle(e => e.idUsuario == idUsuario);

            if (usuario != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario.Preferencias>(usuario.dsPreferencia ?? string.Empty) ?? new Usuario.Preferencias();
            }

            return new Usuario.Preferencias();
        }

        public void SetPreferencias(int idUsuario, Usuario.Preferencias preferencias) 
        {
            Usuario usuario = this.FindSingle(e => e.idUsuario == idUsuario);

            if (usuario != null)
            {
                usuario.dsPreferencia = Newtonsoft.Json.JsonConvert.SerializeObject(preferencias);
                base.Update(usuario);
            }
        }
        #endregion

        #region Foto
        public void SavePhoto(string path, int idUsuario, HttpPostedFileBase file)
        {
            if (file != null)
            {
                RemovePhoto(path, idUsuario);

                string tempName = Path.GetTempFileName();
                file.SaveAs(tempName);

                using (ImageFactory imageFactory = new ImageFactory())
                {
                    imageFactory.
                        Load(tempName).
                        Format(System.Drawing.Imaging.ImageFormat.Jpeg).
                        Quality(100).
                        Resize(150, 0).
                        Save(path + idUsuario + ".jpg");
                }

                using (ImageFactory imageFactory = new ImageFactory())
                {
                    imageFactory.
                        Load(tempName).
                        Format(System.Drawing.Imaging.ImageFormat.Jpeg).
                        Quality(100).
                        Resize(50, 0).
                        Save(path + idUsuario + "_small.jpg");
                }

                File.Delete(tempName);
            }
        }

        public void RemovePhoto(string path, int idUsuario)
        {
            if (System.IO.File.Exists(path + idUsuario + ".jpg"))
            {
                System.IO.File.Delete(path + idUsuario + ".jpg");
            }

            if (System.IO.File.Exists(path + idUsuario + "_small.jpg"))
            {
                System.IO.File.Delete(path + idUsuario + "_small.jpg");
            }

        }

        public System.Drawing.Image LoadPhoto(string path, int idUsuario)
        {
            path += idUsuario + ".jpg";
            if (System.IO.File.Exists(path))
            {
                return System.Drawing.Image.FromFile(path);
            }
            return null;
        }

        public System.Drawing.Image LoadSmallPhoto(string path, int idUsuario)
        {
            path += idUsuario + "_small.jpg";
            if (System.IO.File.Exists(path))
            {
                return System.Drawing.Image.FromFile(path);
            }
            return null;
        }

        //TODO Colocar aqui a converção do path fisico para path virtual para utilizar o parametro tbm
        public string PathPhoto(string path, int idUsuario, bool small)
        {
            if (small)
            {
                path += idUsuario + "_small.jpg";
            }
            else 
            {
                path += idUsuario + ".jpg";
            }
            if (System.IO.File.Exists(path))
            {
                return "/Files/Fotos/Usuarios/" + idUsuario + ".jpg";
            }
            return string.Empty;
        }
        #endregion
    }
}
