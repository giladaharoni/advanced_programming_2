namespace advanced_programming_2.Models
{
    public class chathistory
    {
        public int Id { get; set; }
        //היסטוריית שיחות – הודעות, 2 אנשי קשר.
        public List<message> Messages { get; set; }

        public Contact contact { get; set; }

        
    }
}
