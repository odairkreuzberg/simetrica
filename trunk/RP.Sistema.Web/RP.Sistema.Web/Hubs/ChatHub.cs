using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using RP.Sistema.BLL;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RP.Sistema.Web.Helpers;

namespace RP.Sistema.Web.Hubs
{
    public class ChatUser
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

    public class ChatUserContact
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public ChatStatus Status { get; set; }
    }

    public class ChatUserConfig
    {
        public int UserId { get; set; }
        public ChatStatus Status { get; set; }
        public bool ShowOfflineContacts { get; set; }
        public bool Enabled { get; set; }
        public bool AlertSound { get; set; }
        public bool AlertMessage { get; set; }
    }

    public class ChatMessage
    {
        public int senderId { get; set; }
        public string senderName { get; set; }
        public int receiverId { get; set; }
        public string receiverName { get; set; }
        public string messageText { get; set; }
        public DateTime timestamp { get; set; }
    }

    [Authorize]
    [HubName("RPChatHub")]
    public class RPChatHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ChatUser> _Users = new ConcurrentDictionary<int, ChatUser>();

        private ChatUser GetChatUser(int userId)
        {
            ChatUser user;
            _Users.TryGetValue(userId, out user);
            return user;
        }

        public override Task OnConnected()
        {
            int userId = (Context.User as CustomPrincipal).Id;
            string connectionId = Context.ConnectionId;
            UserConfig config;
            
            var user = _Users.GetOrAdd(userId, _ => new ChatUser
            {
                Id = userId,
                Nome = (Context.User as CustomPrincipal).Nome,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                config = RPChatBLL.GetConfig(userId);

                if (config == null)
                {
                    RPChatBLL.AddConfig(new UserConfig
                    {
                        UserId = userId,
                        Status = ChatStatus.Online,
                        Enabled = true,
                        AlertSound = true
                    });

                    config = RPChatBLL.GetConfig(userId);
                }

                user.ConnectionIds.Add(connectionId);

                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userId, config.Status);
                }
            }

            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Clients.Caller.userReconnected();
            return base.OnReconnected();
        }

        public override Task OnDisconnected()
        {
            int userId = (Context.User as CustomPrincipal).Id;
            string connectionId = Context.ConnectionId;

            ChatUser user = GetChatUser(userId);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {
                        ChatUser removedUser;
                        _Users.TryRemove(userId, out removedUser);

                        Clients.Others.userDisconnected(userId);
                    }
                }
            }

            return base.OnDisconnected();
        }

        public ChatUserConfig GetUserConfig()
        {
            UserConfig config = RPChatBLL.GetConfig((Context.User as CustomPrincipal).Id);

            return new ChatUserConfig { 
                UserId = config.UserId,
                Status = config.Status,
                Enabled = config.Enabled,
                AlertSound = config.AlertSound,
                AlertMessage = config.AlertMessage,
                ShowOfflineContacts = config.ShowOfflineContacts
            };
        }

        public void setUserConfig(ChatUserConfig config)
        {
            // obtem o id do usuario
            int userId = (Context.User as CustomPrincipal).Id;

            // obtem a configuracao do usuario
            UserConfig userConfig = RPChatBLL.GetConfig(userId);

            #region status
            // se o valor informado nao existir na Enum
            if (!Enum.IsDefined(typeof(ChatStatus), config.Status))
            {
                // seta para valor padrao (online)
                config.Status = ChatStatus.Online;
            }

            // converte string para ChatStatus
            ChatStatus chatStatus = (ChatStatus)Enum.Parse(typeof(ChatStatus), config.Status.ToString());

            // seta o status
            userConfig.Status = chatStatus;
            #endregion

            // seta o alerta de som
            userConfig.AlertSound = config.AlertSound;

            // seta o alerta de texto
            userConfig.AlertMessage = config.AlertMessage;

            // seta a exibicao de usuario offline
            userConfig.ShowOfflineContacts = config.ShowOfflineContacts;

            // seta a habilitacao do chat
            userConfig.Enabled = config.Enabled;

            // altera a configuracao do usuario
            RPChatBLL.UpdateConfig(userConfig);

            // notifica cliente que fez requisicao
            Clients.Caller.setConfiguration(config);

            // notifica todos os outros clientes o novo status deste cliente
            Clients.Others.userConnected(userId, chatStatus);
        }

        public List<ChatUserContact> GetUserContacts()
        {
            int userId = (Context.User as CustomPrincipal).Id;
            List<ChatUserContact> contatos;

            using (var db = new RP.Sistema.Model.Context())
            {
                var usuarioBLL = new BLL.UsuarioBLL(db, userId);

                // obtem os contatos do usuario atual
                contatos = usuarioBLL.Search(string.Empty).Select(s => new ChatUserContact
                {
                    Id = s.idUsuario,
                    Nome = s.nmUsuario,
                    Email = s.dsEmail
                })
                .ToList();

                // obtem os usuario conectados ao servidor
                var connectedUsers = _Users.Where(e =>
                {
                    lock (e.Value.ConnectionIds)
                    {
                        return !e.Value.ConnectionIds.Contains(Context.ConnectionId);
                    }
                });

                // percorre os usuarios do servidor
                foreach (var u in connectedUsers)
                { 
                    // se o usuario percorrido estiver dentro dos contatos do usuario atual
                    if (contatos.Any(e => e.Id == u.Value.Id))
                    {
                        // busca o contato e seta seu status com a informacao do banco
                        contatos.Find(e => e.Id == u.Value.Id).Status = RPChatBLL.GetConfig(u.Value.Id).Status;
                    }
                }

                return contatos;
            }
        }

        public List<ChatMessage> GetChatMessages(int receiverId)
        {
            List<ChatMessage> list = new List<ChatMessage>();
            List<BLL.ChatMessage> messages = RPChatBLL.GetChatMessages((Context.User as CustomPrincipal).Id, receiverId);

            foreach (var message in messages)
            {
                list.Add(new ChatMessage{
                    senderId = message.senderId,
                    senderName = message.senderName,
                    receiverId = message.receiverId,
                    receiverName = message.receiverName,
                    messageText = message.messageText,
                    timestamp = message.timestamp
                });
            }

            return list;
        }

        public void SendNotification(int receiverId)
        {
            ChatUser receiver = GetChatUser(receiverId);
            ChatUser sender = GetChatUser((Context.User as CustomPrincipal).Id);

            IEnumerable<string> allReceivers;
            lock (receiver.ConnectionIds)
            {
                lock (sender.ConnectionIds)
                {
                    allReceivers = receiver.ConnectionIds.Concat(sender.ConnectionIds);
                }
            }

            foreach (var cid in allReceivers)
            {
                Clients.Client(cid).receiveNotification(sender.Id, sender.Nome);
            }
        }

        public void SendMessage(string message, int receiverId)
        {
            ChatUser receiver = GetChatUser(receiverId);
            ChatUser sender;
            ChatMessage chatMessage;

            if (receiver != null)
            {
                sender = GetChatUser((Context.User as CustomPrincipal).Id);

                chatMessage = new ChatMessage {
                    senderId = (Context.User as CustomPrincipal).Id,
                    senderName = (Context.User as CustomPrincipal).Nome,
                    receiverId = receiver.Id,
                    receiverName = receiver.Nome,
                    timestamp = DateTime.Now,
                    messageText = message
                };

                IEnumerable<string> allReceivers;
                lock (receiver.ConnectionIds)
                {
                    lock (sender.ConnectionIds)
                    {
                        RPChatBLL.AddChatMessage(new BLL.ChatMessage
                        {
                            senderId = chatMessage.senderId,
                            senderName = chatMessage.senderName,
                            receiverId = chatMessage.receiverId,
                            receiverName = chatMessage.receiverName,
                            messageText = chatMessage.messageText,
                            timestamp = chatMessage.timestamp
                        });

                        allReceivers = receiver.ConnectionIds.Concat(sender.ConnectionIds);
                    }
                }

                foreach (var cid in allReceivers)
                {
                    Clients.Client(cid).receiveMessage(chatMessage);
                }
            }
        }
    }
}