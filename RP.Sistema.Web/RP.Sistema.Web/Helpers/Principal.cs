using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RP.Sistema.Web.Helpers
{
    interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        string Nome { get; set; }
        string Login { get; set; }
        string ConnectionId { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string ConnectionId { get; set; }
    }

    public class CustomPrincipalSerialized
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string ConnectionId { get; set; }
    }
}