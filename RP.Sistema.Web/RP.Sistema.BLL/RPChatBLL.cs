using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace RP.Sistema.BLL
{
    public enum ChatStatus
    {
        Offline = 1,
        Online = 2,
        Busy = 3,
        Away = 4,
        Invisible = 5
    }

    public class ChatMessage
    {
        public ObjectId Id { get; set; }
        
        [BsonRequired]
        [BsonElementAttribute("senderid")]
        public int senderId { get; set; }
        
        [BsonRequired]
        [BsonElementAttribute("sendername")]
        public string senderName { get; set; }

        [BsonRequired]
        [BsonElementAttribute("receiverid")]
        public int receiverId { get; set; }

        [BsonRequired]
        [BsonElementAttribute("receivername")]
        public string receiverName { get; set; }

        [BsonRequired]
        [BsonElementAttribute("text")]
        public string messageText { get; set; }

        [BsonRequired]
        [BsonElementAttribute("date")]
        public DateTime timestamp { get; set; }
    }

    public class UserConfig
    {
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElementAttribute("userid")]
        public int UserId { get; set; }

        [BsonElementAttribute("status")]
        public ChatStatus Status { get; set; }

        [BsonElementAttribute("showofflinecontacts")]
        public bool ShowOfflineContacts { get; set; }

        [BsonElementAttribute("enabled")]
        public bool Enabled { get; set; }

        [BsonElementAttribute("alertsound")]
        public bool AlertSound { get; set; }

        [BsonElementAttribute("alertmessage")]
        public bool AlertMessage { get; set; }

        public UserConfig() 
        {
            Status = ChatStatus.Online;
            ShowOfflineContacts = true;
            Enabled = true;
            AlertSound = true;
            AlertMessage = true;
        }
    }

    public class RPChatBLL
    {
        private static MongoDatabase _database;

        private static MongoDatabase GetDatabase()
        {
            if (_database == null)
            {
                MongoUrl url = new MongoUrl(System.Configuration.ConfigurationManager.ConnectionStrings["MongoServerSettings"].ConnectionString);
                MongoServer server = new MongoServer(MongoServerSettings.FromUrl(url));
                _database = server.GetDatabase(url.DatabaseName);
            }

            return _database;
        }

        public static void AddChatMessage(ChatMessage message)
        {
            var messageCollection = GetDatabase().GetCollection<ChatMessage>("messages");
            messageCollection.Insert(message);
        }

        public static List<ChatMessage> GetChatMessages(int senderId, int receiverId)
        {
            var messageCollection = GetDatabase().GetCollection<ChatMessage>("messages");
            int total = messageCollection.AsQueryable<ChatMessage>()
                .Where(e => (e.senderId == senderId && e.receiverId == receiverId) || (e.receiverId == senderId && e.senderId == receiverId)).Count();

            int take = 40;
            int skip = total > take ? total - take : 0;

            var query = messageCollection.AsQueryable<ChatMessage>()
                .Where(e => (e.senderId == senderId && e.receiverId == receiverId) || (e.receiverId == senderId && e.senderId == receiverId))
                .OrderBy(e => e.timestamp)
                .Skip(skip)
                .Take(take)
                .Select(e => new ChatMessage
                {
                    senderId = e.senderId,
                    senderName = e.senderName,
                    receiverId = e.receiverId,
                    receiverName = e.receiverName,
                    messageText = e.messageText,
                    timestamp = e.timestamp
                });

            return query.ToList();
        }

        public static void AddConfig(UserConfig userConfig) 
        {
            var configCollection = GetDatabase().GetCollection<UserConfig>("configs");
            if (configCollection.AsQueryable<UserConfig>().FirstOrDefault(e => e.UserId == userConfig.UserId) == null)
            {
                configCollection.Insert(userConfig);
            }
        }

        public static void UpdateConfig(UserConfig userConfig)
        {
            var configCollection = GetDatabase().GetCollection<UserConfig>("configs");
            configCollection.Save(userConfig);
        }

        public static UserConfig GetConfig(int userId)
        {
            var configCollection = GetDatabase().GetCollection<UserConfig>("configs");
            return configCollection.AsQueryable<UserConfig>().FirstOrDefault(e => e.UserId == userId) ?? new UserConfig();
        }
    }
}
