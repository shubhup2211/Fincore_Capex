using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace Fincore.Application.DTO
{
        public class ApiResponse<T>
        {
            public bool success { get; set; }
            public string message { get; set; }

            public T? data { get; set; }
            public object? metadata { get; set; }
            public int? totalNumberRecord { get; set; }
            public ApiError? Error { get; set; }

        }
    }
