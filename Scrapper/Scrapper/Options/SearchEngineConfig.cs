namespace Scrapper.Options
{
    public class SearchEngineConfig
    {
        public string Url { get; set; }
        public int RecordsPerRequest { get; set; }
        public int MaxMatchesPerRequest { get; set; }
        public string[] MatchUrls { get; set; }
    }
}
