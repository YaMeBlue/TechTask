namespace TechTask.Models
{
    public class Story
    {
        public int id { get; set; }
        public int time { get; set; }
        public int score { get; set; }
        public int descendants { get; set; }

        public string by { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public List<int> kids { get; set; }
    }
}