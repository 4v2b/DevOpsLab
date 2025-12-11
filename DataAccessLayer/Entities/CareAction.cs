namespace DataAccessLayer.Entities;

public class CareAction : BaseEntity
{
    public Plant AssociatedPlant { get; set; }
    
    public CareActionType ActionType { get; set; }
}