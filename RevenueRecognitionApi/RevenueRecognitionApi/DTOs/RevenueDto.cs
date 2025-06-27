namespace RevenueRecognitionApi.DTOs;

public class RevenueRequestDto
{
    public int? ProductId { get; set; }
    public string? Currency { get; set; }
}

public class RevenueResponseDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public decimal ExchangeRate { get; set; }
}