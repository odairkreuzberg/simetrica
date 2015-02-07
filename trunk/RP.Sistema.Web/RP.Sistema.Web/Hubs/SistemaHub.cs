using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RP.Sistema.Web.Hubs
{
    [Authorize]
    [HubName("SistemaHub")]
    public class SistemaHub : Hub
    {
        public void Relatorio(string text)
        {
            Clients.All.relatorioNotificacao(text);
        }
    }
}