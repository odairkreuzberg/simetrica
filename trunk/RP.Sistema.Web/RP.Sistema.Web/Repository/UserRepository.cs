using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace RP.Sistema.Web.Repository
{
    public class UserInfo
    {
        public string nrSessionID {get; set;}
        public string dsLogin {get; set;}
        public int idUsuario { get; set; }
        public string nmUsuario {get; set;}
        public string nrIP { get; set; }
    }

    public static class UserRepository
    {
        private static List<UserInfo> _listUser = new List<UserInfo>();

        static UserRepository() 
        {
            //recover();
        }

        public static void add(UserInfo info)
        {
            if (_listUser.Count(u => u.nrSessionID == info.nrSessionID) == 0)
            {
                _listUser.Add(info);
            }
        }

        public static void remove(string SessionID)
        {
            _listUser.RemoveAll(u => u.nrSessionID == SessionID);
        }

        public static List<UserInfo> get { get { return _listUser; } }

        internal static void recover()
        {
            if (System.Web.HttpContext.Current.Session["UserRepository"] != null)
                _listUser = (List<UserInfo>)System.Web.HttpContext.Current.Session["UserRepository"];
        }

        internal static void save() 
        {
            System.Web.HttpContext.Current.Session.Add("UserRepository", _listUser);
        }
    }

}
