namespace BuildingBlocks.Domain.Model;

public abstract class Auditable : IAuditable
{
    public DateTime? CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public long? LastModifiedBy { get; set; }
}
