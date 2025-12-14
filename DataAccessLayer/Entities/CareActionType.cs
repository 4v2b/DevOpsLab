namespace DataAccessLayer.Entities;

public class CareActionType : BaseEntity
{
    public string Name { get; set; }

    public List<CareAction> CareActions { get; set; }
}