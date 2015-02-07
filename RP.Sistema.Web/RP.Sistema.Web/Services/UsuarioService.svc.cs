using System;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using RP.Sistema.BLL;
using RP.Sistema.Model;
using RP.Sistema.Model.Entities;
using RP.Sistema.Web.Models.Usuario;

namespace RP.Sistema.Web.Services
{
    public class UsuarioService : IUsuarioService
    {
        public List<UsuarioModel> ListarUsuariosJson()
        {
            return ListarUsuarios();
        }

        public List<UsuarioModel> ListarUsuariosXml()
        {
            return ListarUsuarios();
        }

        private List<UsuarioModel> ListarUsuarios()
        {
            List<UsuarioModel> usuarios = new List<UsuarioModel>();

            try
            {
                using (Context db = new Context())
                {
                    UsuarioBLL bll = new UsuarioBLL(db, -999);
                    foreach (var usuario in bll.Find())
                    {
                        usuarios.Add(new UsuarioModel { 
                            idUsuario = usuario.idUsuario,
                            nmUsuario = usuario.nmUsuario,
                            dsLogin = usuario.dsLogin,
                            dsEmail = usuario.dsEmail,
                            dtValidade = usuario.dtValidade,
                            flAtivo = usuario.flAtivo
                        });
                    }

                    return usuarios;
                }
            }
            catch
            {
                return null;
            }
        }

        public UsuarioModel BuscarUsuarioJson(string idUsuario)
        {
            return BuscarUsuario(idUsuario);
        }

        public UsuarioModel BuscarUsuarioXml(string idUsuario)
        {
            return BuscarUsuario(idUsuario);
        }

        private UsuarioModel BuscarUsuario(string idUsuario)
        {
            int id = Convert.ToInt32(idUsuario);

            try
            {
                using (Context db = new Context())
                {
                    UsuarioBLL bll = new UsuarioBLL(db, -999);
                    //Usuario usuario = bll.FindSingle(u => u.idUsuario == id, p => p.Perfis);
                    Usuario usuario = bll.FindSingle(u => u.idUsuario == id, i => i.Perfis.Select(e => e.Perfil));
                    
                    if (usuario != null)
                    {
                        List<UsuarioModel.Perfil> Perfis = new List<UsuarioModel.Perfil>();

                        foreach (var perfilUsuario in usuario.Perfis)
                        {
                            Perfis.Add(new UsuarioModel.Perfil { 
                                idPerfil = perfilUsuario.idPerfil,
                                nmPerfil = perfilUsuario.Perfil.nmPerfil
                            });
                        }

                        return new UsuarioModel
                        {
                            idUsuario = usuario.idUsuario,
                            nmUsuario = usuario.nmUsuario,
                            dsLogin = usuario.dsLogin,
                            dsEmail = usuario.dsEmail,
                            dtValidade = usuario.dtValidade,
                            flAtivo = usuario.flAtivo,
                            Perfis = Perfis
                        };
                    }

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public void AtualizarPerfisJson(Usuario usuario)
        {
            AtualizarPefis(usuario);
        }

        public void AtualizarPerfisXml(Usuario usuario)
        {
            AtualizarPefis(usuario);
        }

        private void AtualizarPefis(Usuario usuario)
        {
            try
            {
                if (usuario == null || usuario.idUsuario == 0)
                {
                    throw new Exception("Informe o usuário");
                }

                using (Context db = new Context())
                {
                    using (var ts = new RP.DataAccess.RPTransactionScope(db))
                    {

                        UsuarioBLL bll = new UsuarioBLL(db, -999);
                        bll.UpdatePerfis(usuario);

                        db.SaveChanges();
                        ts.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                System.ServiceModel.Web.WebOperationContext ctx = System.ServiceModel.Web.WebOperationContext.Current;
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                ctx.OutgoingResponse.StatusDescription = e.Message;
               // throw new System.ServiceModel.Web.WebFaultException<string>(e.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
