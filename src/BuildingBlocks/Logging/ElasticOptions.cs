namespace BuildingBlocks.Logging;

public class ElasticOptions
{
    public bool Enable { get; set; }
    public string ElasticServiceUrl { get; set; }
    public string ElasticSearchIndex { get; set; }
}