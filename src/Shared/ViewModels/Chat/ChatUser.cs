using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.ViewModels.Chat
{
    public class ChatUser : LongKeyedEntity
    {
        [Required, MaxLength(120)]
        public string Name { get; set; } = "";
    }
}
