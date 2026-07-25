using Fincore.Application.DTO;

namespace Fincore.Infrastructure.CommonHelper
{
    public class ApiResponseHelper
    {
        public static ApiResponse<T> SuccessRes<T>(
            T data,
            string message = "Success",
            int? totalRecord = null,
            object? metadata = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Metadata = metadata,
                TotalNumberRecord = totalRecord
            };
        }

        public static ApiResponse<T> Failure<T>(
            string message,
            string errorCode,
            string details)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Error = new ApiError
                {
                    Code = errorCode,
                    Details = details
                }
            };
        }
    }
}