namespace Fincore.Application.DTO
{
    public class ApiResponse<T>
    {
        
        public bool success { get; set; }

        public string message { get; set; } = string.Empty;

        public T? data { get; set; }

        public object? metadata { get; set; }

        public int? totalNumberRecord { get; set; }

        public ApiError? error { get; set; }
    }
}