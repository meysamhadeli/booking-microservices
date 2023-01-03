namespace BuildingBlocks.Logging;

public class ElasticOptions
{
    public bool Enabled { get; set; }
    public string ElasticServiceUrl { get; set; }
    public string ElasticSearchIndex { get; set; }
}
