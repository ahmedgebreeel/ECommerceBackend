namespace ECommerce.Business.DTOs.Errors
{
    public class ApiErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public string? Detail { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
