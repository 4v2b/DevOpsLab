namespace DataAccessLayer.Entities;

public class Observation : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public string ObservationType { get; set; } // ManualEntry, AutomatedPhoto, SensorReading
    public string Source { get; set; } // UserApp, WateringSystem, CVService
    public string? DataPayload { get; set; } // JSON
    public string? MediaUrl { get; set; }

    public int PlantId { get; set; }
    public Plant Plant { get; set; }

    public int HealthAssessmentId { get; set; }
    public HealthAssessment HealthAssessment { get; set; }
}