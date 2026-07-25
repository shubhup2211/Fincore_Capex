//using Fincore.Application.DTO;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Fincore.Infrastructure.CommonHelper
//{
//    public class ApiResponseHelper
//    {
//        public static Application.DTO.ApiResponse<T> SuccessRes<T>(T Data, string Message = "Success", int? TotalRecord = null, object? Metadata = null)
//        {
//            return new Application.DTO.ApiResponse<T>
//            {
//                success = true,
//                message = Message,
//                data = Data,
//                metadata = Metadata,
//                totalNumberRecord = TotalRecord
//            };
//        }

//        public static Application.DTO.ApiResponse<T> Failure<T>(string Message, string ErrorCode, string Details)
//        {
//            return new Application.DTO.ApiResponse<T>
//            {
//                success = false,
//                message = Message,
//                Error = new ApiError
//                {
//                    code = ErrorCode,
//                    details = Details
//                }
//            };


//        }

//    }

//}
