using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.ViewModels.Chat
{
    public class ChatMessage : LongKeyedEntity
    {
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required, MaxLength(4000)]
        public string Text { get; set; } = "";
    }
}
