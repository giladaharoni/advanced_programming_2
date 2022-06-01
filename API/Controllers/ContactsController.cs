using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using advanced_programming_2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace advanced_programming_2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        public IConfiguration _configuration;

        public ContactsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static List<Contact> _contacts = new List<Contact>() { new Contact() { Id = "1", profileImage = null, LastSeen = DateTime.Now, password = "12341234", username = "didi", nickname = "D", Contacts = null, chathistories = null }, new Contact() { Id = "2", profileImage = null, LastSeen = DateTime.Now, password = "12341234", username = "do", nickname = "o", Contacts = new List<Contact>() { new Contact() { Id = "3", profileImage = null, LastSeen = DateTime.Now, password = "12341234", username = "dodi", nickname = "D", Contacts = null, chathistories = null } }, chathistories = null } };
        // GET: Contacts
        [HttpGet]
        public IEnumerable<Contact> index()
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username==name);
            return finds.Contacts;
            //return _contacts;
        }

        [HttpGet("{id}")]
        // GET: Contacts/Details/5
        public Contact Details(string id)
        {


            return _contacts.Where(x => x.Id == id).FirstOrDefault();
        }



        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public void Create(string id, string name, string server)
        {
            var contact = _contacts.Find(e => e.Id == id);
            var user = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username == user);
            if(finds.Contacts == null)
            {
                finds.Contacts = new List<Contact>();
            }
            if(contact != null)
            {
                if (contact.Id != finds.Id)
                {
                    finds.Contacts.Add(contact);
                }
            }
            
        }

        [HttpGet("{id}/messages")]
        public List<message> Messages(string id)
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username == name);
            if (finds.chathistories == null)
            {
                finds.chathistories = new List<chathistory>();
                finds.chathistories.Add(new chathistory { Messages = new List<message>(), contact = _contacts[1], Id = 3 });
            }
            var mchathistory = finds.chathistories.ToList();
            chathistory mcontact = null;
            foreach (chathistory a in mchathistory.ToList())
            {
                if (a.contact.Id == id)
                {
                    mcontact = a;
                }
            }
            if (mcontact != null)
            {
                return mcontact.Messages;

            }
            else return null;
        }

        [HttpPost("{id}/messages")]
        public void Message(string id ,message message)
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var firstuser = _contacts.Find(e => e.username == name);
            var lastuser = _contacts.Find(e => e.Id == id);
            firstuser.chathistories.ToList().Find(e => e.contact == lastuser).Messages.Add(message);
            lastuser.chathistories.ToList().Find(e => e.contact == firstuser).Messages.Add(message);

        }

        [HttpDelete("{id}")]
        // GET: Contacts/Details/5
        public void deleteContact(string id)
        {
            var remcon = _contacts.Where(x => x.Id == id).FirstOrDefault();
            foreach(Contact contact in _contacts)
            {
                if (contact.Contacts.Contains(remcon))
                {
                    contact.Contacts.Remove(remcon);
                }
            }
            _contacts.Remove(remcon);
        }


        [Route("{id?}/messages/{id2?}")]
        [HttpGet]
        public message GetMessage(string id, int id2)
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username == name);
            if (finds.chathistories == null)
            {
                finds.chathistories= new List<chathistory>();
                return null;
            }
            var mchathistory = finds.chathistories.ToList();
            var history = mchathistory.Find(e => e.contact.Id == id);
            if (history != null)
            {
                var mess = history.Messages.Find(e => e.Id == id2);
                if (mess != null)
                {
                    return mess;
                }
            }

            return null;
        }



        [Route("{id?}/messages/{id2?}")]
        [HttpPut]
        public void updateMessage(string id, int id2, message message)
        {
            var mess = GetMessage(id, id2);
            if (mess != null)
            {
                mess.sendTime = message.sendTime;
                mess.isOneSend = message.isOneSend;
                mess.content = message.content;
            }
        }

        [Route("{id?}/messages/{id2?}")]
        [HttpDelete]
        public void deleteMessage(string id ,int id2)
        {
            if (GetMessage(id, id2) != null)
            {
                var name = HttpContext.User.Claims.ToList()[3].Value;
                var finds = _contacts.Find(e => e.username == name);
                finds.chathistories.ToList().Find(e => e.contact.Id==id).Messages.Remove(GetMessage(id,id2));

            }
        }

    }
}

