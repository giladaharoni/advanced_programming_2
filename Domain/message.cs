namespace advanced_programming_2.Models
{
    public class message
    {
        public static int counter=0;

        //שעה, תוכן, מי שלח, מי קיבל.
        public int Id { get; set; }
        public DateTime created { get; set; }

        public string content { get; set; }

        public bool sent { get; set; }

        public message(string content, bool isOneSend)
        {
            this.content = content;
            this.sent = isOneSend;
            created = DateTime.Now;
            Id = ++counter;
        }
    }
    
}
