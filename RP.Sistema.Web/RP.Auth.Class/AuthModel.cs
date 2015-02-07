using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RP.Auth.Class
{
    public static class AuthModel
    {
        public class users
        {
            public string user { get; set; }
            public string key { get; set; }
        }

        public class permissao
        {
            public string modulo { get; set; }
            public string area { get; set; }
            public string controle { get; set; }
            public string acao { get; set; }
            public bool flarea { get; set; }
        }

        private static List<users> _usersDB = new List<users>();
        private static Dictionary<string, List<permissao>> _rigthsDB = new Dictionary<string, List<permissao>>();

        public static bool check(string login, string key, permissao p)
        {
            users _u = new users();
            _u.user = login.ToLower().Trim();
            _u.key = key;

            if (!_usersDB.Any<users>(item => item.user == _u.user && item.key == _u.key))
            {
                Remove(_u.user);
                Load(_u);
            }

            return Verify(_u.user, p);
        }

        public static bool Verify(string login, permissao p)
        {
            if (_rigthsDB.ContainsKey(login))
            {
                List<permissao> pDB = _rigthsDB[login];
                foreach (permissao item in pDB)
                {
                    var _acao = item.acao.ToLower().Trim();
                    if (_acao.IndexOf('?') > 0)
                    {
                        _acao = _acao.Substring(0, _acao.IndexOf('?'));
                    }
                    if (item.controle.ToLower().Trim() == p.controle.ToLower().Trim() &&
                        _acao == p.acao.ToLower().Trim())
                    {
                        if (item.flarea)
                        {
                            if (item.area.ToLower().Trim() == p.area.ToLower().Trim())
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool Verify(string login, string modulo, string area, string controle, string acao)
        {
            var p = new permissao
            {
                acao = acao,
                area = area,
                controle = controle,
                flarea = true,
                modulo = modulo
            };
            return Verify(login, p);
        }

        private static void Load(users user)
        {
            RP.Sistema.Model.Context db = new Sistema.Model.Context();

            var p  = from perfil in db.Perfis
                     from perfilUsuario in perfil.Usuarios
                     from perfilacao in perfil.Acoes
                     join acao in db.Acoes on perfilacao.idAcao equals acao.idAcao
                     join controle in db.Controles on acao.idControle equals controle.idControle
                     join area in db.Areas on controle.idArea equals area.idArea
                     join modulo in db.Modulos on area.idModulo equals modulo.idModulo
                     join usuario in db.Usuarios on perfilUsuario.idUsuario equals usuario.idUsuario
                     where (usuario.dsLogin == user.user && usuario.flAtivo == "Sim")                     
                     select (new permissao { modulo = modulo.nmURL, area = area.nmArea, controle = controle.nmControle, acao = acao.nmAcao, flarea = (area.flUsaURL.ToLower() == "sim") });

            _rigthsDB.Add(user.user, p.ToList());

            _usersDB.Add(user);
        }

        public static void Remove(string user)
        {
            users _u = null;
            foreach (users item in _usersDB)
            {
                if (item.user.ToLower().Trim() == user)
                {
                    _u = item;
                    break;
                }
            }

            if (_u != null)
            {
                _usersDB.Remove(_u);
                _rigthsDB.Remove(user);
            }
        }


    }
}