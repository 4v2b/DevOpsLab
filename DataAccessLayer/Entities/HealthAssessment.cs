namespace DataAccessLayer.Entities;

public class HealthAssessment : BaseEntity
{
    public string AssessmentSource { get; set; } // CVService, UserInput
    public string HealthStatus { get; set; } // Healthy, Warning, Critical
    public decimal? HealthScore { get; set; }
    public string DetectedIssues { get; set; } // JSON array
    public decimal? ConfidenceLevel { get; set; }
    public string Recommendations { get; set; }

    public int ObservationId { get; set; }
    public Observation Observation { get; set; }
}