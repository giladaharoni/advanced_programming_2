using advanced_programming_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class viewContact
    {
        public string id { get; set; }

        public DateTime? lastDate { get; set; }

        public string last { get; set; }

        public string server { get; set; }

        public string name { get; set; }

        public viewContact(Contact contact)
        {
            id = contact.Id;
            lastDate = contact.LastSeen;
            server = contact.server;
            name = contact.nickname;
            last = "";
        } 
    }
}
