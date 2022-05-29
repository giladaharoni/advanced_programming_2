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
        private static List<Contact> _contacts = new List<Contact>() { new Contact() { Id = "1", profileImage = null, LastSeen = DateTime.Now, password = "12341234", username = "didi", nickname = "D", Contacts = new List<Contact>(), chathistories = null }, };

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
            finds.Contacts.Add(contact);
        }

    }
}

