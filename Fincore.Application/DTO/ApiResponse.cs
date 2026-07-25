namespace Fincore.Application.DTO
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public object? Metadata { get; set; }

        public int? TotalNumberRecord { get; set; }

        public ApiError? Error { get; set; }
    }
}