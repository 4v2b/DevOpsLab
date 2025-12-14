namespace DataAccessLayer.Entities;

public class CareAction : BaseEntity
{
    public DateTime ExecutedAt { get; set; }
    public string Executor { get; set; }
    public string Parameters { get; set; } // JSON
    public string TriggeredBy { get; set; }

    public int PlantId { get; set; }
    public Plant Plant { get; set; }

    public int CareActionTypeId { get; set; }
    public CareActionType CareActionType { get; set; }
}