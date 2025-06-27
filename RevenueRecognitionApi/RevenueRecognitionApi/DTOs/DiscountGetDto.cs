namespace RevenueRecognitionApi.DTOs;

public class DiscountGetDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public decimal Percentage { get; set; }
    
    public DateTime Start { get; set; }
    
    public DateTime End { get; set; }
    
    public int SoftwareProductId { get; set; }
}