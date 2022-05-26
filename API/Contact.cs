namespace advanced_programming_2.Models
{
    public class Contact
    {
        public int Id { get; set; }
        //public virtual ICollection<Contact>? Contacts { get; set; }
        public string profileImage { get; set; }

        public DateTime? LastSeen { get; set; }

        public string password { get; set; }

        public string username { get; set; }

        public string nickname { get; set; }

        //public virtual ICollection<chathistory> chathistories { get; set; }


    }


}
