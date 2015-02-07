using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace RP.Auth.Class
{
    public static class License
    {
        public class Info
        {
            public bool isLogged { get; set; }
            public DateTime? dtLogin { get; set; }
            public string dsLogin { get; set; }
            public int nrOcor { get; set; }
            public TimeSpan dtTotalTime { get; set; }
            public string idSession { get; set; }
            public DateTime? dtSignal { get; set; }
        }

        private static int __limit = 0;
        private static System.Collections.Hashtable __active = new System.Collections.Hashtable();

        static License() 
        {
            string _license = System.Configuration.ConfigurationManager.AppSettings["License"];
            if (!string.IsNullOrEmpty(_license))
            {
                __limit = int.Parse(_license);
            }
        }

        public static void SetSignal()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string nrAcesso = GetNrAcesso();
                if (!string.IsNullOrEmpty(nrAcesso))
                {
                    if (__active.ContainsKey(nrAcesso))
                    {
                        Info _info = (Info)__active[nrAcesso];
                        _info.dtSignal = DateTime.Now;
                    }
                    else
                    {
                        __active.Add(nrAcesso, new Info { isLogged = true, dsLogin = string.Empty, dtLogin = null, nrOcor = 0, dtTotalTime = new TimeSpan(), idSession = nrAcesso });
                    }
                }
            }
            GarbageCollect();
        }

        private static string GetNrAcesso()
        {
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ticket.UserData);
                return obj.ConnectionId;
            }
            else 
            {
                return string.Empty;
            }
        }

        public static void Register(string nrAcesso = "")
        {
            if (string.IsNullOrEmpty(nrAcesso))
            { 
                nrAcesso = GetNrAcesso();            
            }

            if (!__active.ContainsKey(nrAcesso))
            {
                try
                {
                    __active.Add(nrAcesso, new Info { isLogged = false, dsLogin = string.Empty, dtLogin = null, nrOcor = 0, dtTotalTime = new TimeSpan(), idSession = nrAcesso });
                }
                catch (Exception)
                {
                    Console.WriteLine("Erro ao adicionar o Nº de acesso [" +  nrAcesso + "]");
                }
            }
            else
            {
                Info _info = (Info)__active[nrAcesso];

                _info.nrOcor = _info.nrOcor + 1;
                _info.isLogged = false;
                _info.dtLogin = null;
                try
                {
                    __active[nrAcesso] = _info;
                }
                catch (Exception)
                {
                    Console.WriteLine("Erro ao atualizar o Nº de acesso [" + nrAcesso + "]");
                }
            }
        }

        public static bool UseLicense(string Login, string guid = "")
        {
            string nrAcesso;
            if (string.IsNullOrEmpty(guid))
            {
                nrAcesso = GetNrAcesso();
            }
            else
            {
                nrAcesso = guid;
            }

            if (!__active.ContainsKey(nrAcesso))
            {
                Register(nrAcesso);
            }

            if (HaveLicense())
            {
                Info _info = (Info)__active[nrAcesso];
                if (!_info.isLogged)
                {
                    _info.isLogged = true;
                    _info.dsLogin = Login;
                    _info.dtLogin = DateTime.Now;
                    _info.idSession = nrAcesso;
                }

                _info.dtSignal = DateTime.Now;
                __active[nrAcesso] = _info;

                return true;
            }

            return false;
        }

        public static void UnlockLicense(string nrAcesso)
        {
            if (__active.ContainsKey(nrAcesso))
            {
                __active.Remove(nrAcesso);
            }
        }

        public static void kill(string nrAcesso)
        {
            __active.Remove(nrAcesso);
        }

        public static void Logout()
        {
            string nrAcesso = GetNrAcesso();

            if (__active.ContainsKey(nrAcesso))
            {
                Info _info = (Info)__active[nrAcesso];

                _info.dtLogin = null;
                _info.isLogged = false;
            }
            else
            {
                Register(nrAcesso);
            }
        }

        public static void SessionEnd()
        {
            string nrAcesso = GetNrAcesso();

            if (__active.ContainsKey(nrAcesso))
            {
                Info _info = (Info)__active[nrAcesso];

                _info.dtLogin = null;
                _info.dtSignal = null;
                _info.isLogged = false;
            }
            else
            {
                Register(nrAcesso);
            }        
        }

        public static bool HaveLicense()
        {
            if (__limit > 0)
            {
                return (__limit > CountLogged());
            }
            return true;
        }

        private static int CountLogged() 
        {
            var _enum = __active.GetEnumerator();
            int _result = 0;

            while (_enum.MoveNext())
            { 
                Info _info = (Info)_enum.Value;
                if (_info.isLogged)
                {
                    _result = _result + 1;
                }
            }
            return _result;
        }

        public static List<Info> Get(bool onlyLogged = false)
        {
            List<Info> _result = new List<Info>();
            var _enum = __active.GetEnumerator();

            while (_enum.MoveNext())
            {
                Info _info = (Info)_enum.Value;
                if (onlyLogged && _info.isLogged)
                {
                    _result.Add(_info);
                }
                else if(!onlyLogged)
                {
                    _result.Add(_info);
                }
            }

            return _result;
        }

        private static void GarbageCollect() 
        {
            List<string> keys = __active.Keys.Cast<string>().ToList() ;

            foreach (var key in keys)
            {
                Info _info = (Info)__active[key];

                if (_info.dtSignal != null)
                {
                    if (_info.dtSignal.Value < DateTime.Now.AddMinutes(-3))
                    {
                        __active.Remove(key);
                    }
                }   
            }

        } 

    }
}
