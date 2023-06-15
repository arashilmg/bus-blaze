namespace BizCover.Application.Renewals.DTO;

public class RenewalDatesDto
{
    public DateTime Initiation { get; init; }
    public DateTime? Initiated { get; set; }
    public DateTime OrderGeneration { get; init; }
    public DateTime? OrderGenerated { get; set; }
    public DateTime OrderSubmission { get; init; }
    public DateTime? OrderSubmitted { get; set; }
}
