namespace BuildingBlocks.Logging
{
    public class LogOptions
    {
        public string Level { get; set; }
        public ElasticOptions Elastic { get; set; }
        public FileOptions File { get; set; }
        public string LogTemplate { get; set; }
    }
}