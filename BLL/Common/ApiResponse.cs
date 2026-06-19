using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; } 

        public bool IsSuccess { get; set; }

        public string? Message { get; set; } 

        public List<string>? Errors { get; set; }

        public int  StatusCode { get; set; }
    }
}
