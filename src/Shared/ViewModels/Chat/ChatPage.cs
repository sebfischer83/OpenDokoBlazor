using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenDokoBlazor.Shared.ViewModels.Chat
{
    public class ChatPage
    {
        [JsonIgnore]
        public long? MinMessageId { get; }
        [JsonIgnore]
        public long? MaxMessageId { get; }
        // Must be sorted by ChatMessage.Id
        public List<ChatMessage> Messages { get; }
        public Dictionary<long, ChatUser> Users { get; }

        public ChatPage()
            : this(new List<ChatMessage>(), new Dictionary<long, ChatUser>()) { }
        [JsonConstructor]
        public ChatPage(List<ChatMessage> messages, Dictionary<long, ChatUser> users)
        {
            Messages = messages;
            Users = users;
            if (messages.Count > 0)
            {
                MinMessageId = messages.Min(m => m.Id);
                MaxMessageId = messages.Max(m => m.Id);
            }
        }
    }
}