namespace advanced_programming_2.Models
{
    public class message
    {
        //שעה, תוכן, מי שלח, מי קיבל.
        public int Id { get; set; }
        public DateTime sendTime { get; set; }

        public string content { get; set; }

        public Contact sender { get; set; }

        public Contact reciever { get; set; }
    }
}
