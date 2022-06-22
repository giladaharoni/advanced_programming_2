using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using advanced_programming_2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Domain;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;

namespace advanced_programming_2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        static int counter = 20;

        public IConfiguration _configuration;

        public ContactsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static List<Contact> _contacts = new List<Contact>()
        { new Contact() { Id = "do@gmail.com", profileImage = null, LastSeen = DateTime.Now, password = "1", username = "do", nickname = "didi", Contacts = new List<Contact>() , chathistories = new List<chathistory>() } ,
            new Contact() { Id = "dodi@gmail.com", profileImage = null, LastSeen = DateTime.Now, password = "1", username = "dodi", nickname = "dodi", Contacts = new List<Contact>() , chathistories = new List<chathistory>() } ,
            new Contact() { Id = "dori@gmail.com", profileImage = null, LastSeen = DateTime.Now, password = "1", username = "dori", nickname = "dori", Contacts = new List<Contact>() , chathistories = new List<chathistory>() } };

        private static List<String> Tokens = new List<string>();
        // GET: Contacts
        [HttpGet]
        public List<viewContact> index()
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username==name);
            List<viewContact> contactViewList = new List<viewContact>();
            foreach (var contact in finds.Contacts)
            {
                
                    if (finds.chathistories != null)
                    {
                        if (finds.chathistories.ToList().Find(k => k.contact == contact)?.Messages != null)
                            contactViewList.Add(new viewContact(contact) { last = finds.chathistories.ToList().Find(k => k.contact == contact).Messages.ToList().Last().content });
                        else
                            contactViewList.Add(new viewContact(contact));
                    }
                    else
                        contactViewList.Add(new viewContact(contact));
                
            }
            return contactViewList;
        }

        [HttpGet("{id}")]
        // GET: Contacts/Details/5
        public viewContact Details(string id)
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username == name);
            Contact contact = finds.Contacts.Where(x => x.Id == id).FirstOrDefault();
            if (contact == null)
            {
                return null;
            }
          
           var view = new viewContact(contact) { last = finds?.chathistories?.ToList()?.Find(k => k.contact == contact)?.Messages?.ToList()?.Last()?.content };

            return view;
        }

        public static bool Login(string username, string password)
        {
            if (_contacts.Find(e => e.password == password && e.username == username) != null) { return true; }
            return false;
            
        }

        public static bool register(string username,string nickname, string password)
        {
            if (Login(username, password))
            {
                return false;
            }
            Contact contact = (new Contact() { username = username, password = password, nickname = nickname, Id = username });
            counter++;
            contact.chathistories = new List<chathistory>();
            contact.Contacts = new List<Contact>();
            _contacts.Add(contact);
            
            return true;

        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async void Create([Bind("id,name,server")] contactPost conta)
        {
            var contact = _contacts.Find(e => e.Id == conta.id);
            var user = HttpContext.User.Claims.ToList()[3].Value;
            var finds = _contacts.Find(e => e.username == user);
            if(finds.Contacts == null)
            {
                finds.Contacts = new List<Contact>();
            }
            if (finds.Contacts.Contains(contact)) {
                return;
            }
            if (contact != null)
            {
                if (contact.Id != finds.Id)
                {
                    finds.Contacts.Add(contact);
                    contact.Contacts.Add(finds);
                }
            }
            FirebaseApp.Create(
                new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            var m = new MulticastMessage()
            {
                Tokens = Tokens,
                Data = new Dictionary<String, String>() { { "1", "1" } }
            };
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(m);

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
        public async Task MessageAsync(string id ,messagePost messageP)
        {
            //find the user connected
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var firstuser = _contacts.Find(e => e.username == name);
            firstuser.LastSeen = DateTime.Now;
            //find the reciever
            var lastuser = _contacts.Find(e => e.Id == id);
            var myMessage = new message(messageP.content, true);
            var hisMessage = new message(messageP.content, false);
            if (firstuser.chathistories == null)
            {
                firstuser.chathistories = new List<chathistory>() { new chathistory() { contact = lastuser, Id = myMessage.Id, Messages = new List<message>() { myMessage } } };
            } else
            {
                var histo = firstuser.chathistories.ToList().Find(e => e.contact == lastuser);
                if (histo != null)
                {
                    if (histo.Messages != null)
                    {
                        histo.Messages.Add(myMessage);
                    } else
                    {
                        histo.Messages = new List<message> { myMessage };
                    }
                } else
                {
                    firstuser.chathistories.Add(new chathistory() { contact = lastuser, Id = myMessage.Id, Messages = new List<message> { myMessage } });
                }
            }
            if (lastuser.chathistories == null)
            {
                lastuser.chathistories = new List<chathistory>() { new chathistory() { contact = firstuser, Id = hisMessage.Id, Messages = new List<message>() { hisMessage } } };
            } else
            {
                var chahist = lastuser.chathistories.ToList().Find(e=>e.contact == firstuser);
                if (chahist != null)
                {
                    chahist.Messages.Add(hisMessage);
                }
                else
                {
                    lastuser.chathistories.Add(new chathistory() { Messages = new List<message> { hisMessage },contact=firstuser,Id=hisMessage.Id });
                }
            }

            FirebaseApp.Create(
            new AppOptions()
            {
            Credential = GoogleCredential.FromFile("private_key.json")
            });
            var m = new MulticastMessage()
            {
                Tokens = Tokens,
                Data = new Dictionary<String, String>() { { "1", "1" } }
            };
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(m);

        }

        [HttpDelete("{id}")]
        // GET: Contacts/Details/5
        public void deleteContact(string id)
        {
            var name = HttpContext.User.Claims.ToList()[3].Value;
            var firstuser = _contacts.Find(e => e.username == name);
            var remcon = _contacts.Where(x => x.Id == id).FirstOrDefault();
            if (firstuser.Contacts != null)
            {
                firstuser.Contacts.Remove(remcon);
                var chat = firstuser.chathistories?.ToList()?.Find(e => e.contact == remcon);
                if (chat != null)
                {
                    firstuser.chathistories.Remove(chat);
                }
            }


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
        public void updateMessage(string id, int id2, messagePost message)
        {
            var mess = GetMessage(id, id2);
            if (mess != null)
            {
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

