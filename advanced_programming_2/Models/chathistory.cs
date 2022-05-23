namespace advanced_programming_2.Models
{
    public class chathistory
    {
        //היסטוריית שיחות – הודעות, 2 אנשי קשר.
        public List<message> messages { get; set; }

        public Contact contact1 { get; set; }

        public Contact Contact2 { get; set; }
    }
}
