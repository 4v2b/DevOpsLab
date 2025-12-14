namespace DataAccessLayer.Entities;

public class Plant : BaseEntity
{
    public string Name { get; set; }
    public string Species { get; set; }
    public string Location { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Status { get; set; }
    
    public List<CareAction> CareActions { get; set; }
    public List<Observation> Observations { get; set; }
}