namespace TechTask.Configurations
{
    public class HackerNewsConfiguration
    {
        public string BaseUrl { get; set; }

        public string BestStoriesUrl { get; set; }

        public string SpecificStoryUrl { get; set; }

        public int MaxRequestPerSecond { get; set; }
    }
}