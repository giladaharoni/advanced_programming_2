namespace advanced_programming_2.Models
{
    public class message
    {
        public static int counter=0;

        //שעה, תוכן, מי שלח, מי קיבל.
        public int Id { get; set; }
        public DateTime sendTime { get; set; }

        public string content { get; set; }

        public bool isOneSend { get; set; }

        public message(string content, bool isOneSend)
        {
            this.content = content;
            this.isOneSend = isOneSend;
            sendTime = DateTime.Now;
            Id = ++counter;
        }
    }
    
}
