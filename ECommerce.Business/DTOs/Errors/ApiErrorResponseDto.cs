namespace ECommerce.Business.DTOs.Errors
{
    public class ApiErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
