using advanced_programming_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class messageView
    {
        public int id { get; set; }
        public DateTime created { get; set; }

        public string content { get; set; }

        public bool sent { get; set; }

        messageView(message mesasge)
        {
            this.id = mesasge.Id;
            this.created = mesasge.sendTime;
            this.content = mesasge.content;
            this.sent = mesasge.isOneSend;
        }
    }
}
