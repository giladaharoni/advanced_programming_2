namespace advanced_programming_2.Models
{
    public class chathistory
    {
        public int Id { get; set; }
        //היסטוריית שיחות – הודעות, 2 אנשי קשר.
        public ICollection<message> messages { get; set; }

        public Contact contact1 { get; set; }

        public Contact Contact2 { get; set; }
    }
}
