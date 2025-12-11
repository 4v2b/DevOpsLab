namespace DataAccessLayer.Entities;

public class Plant
{
    public List<CareAction> CareActions { get; set; }
    public List<Observation> Observations { get; set; }
}