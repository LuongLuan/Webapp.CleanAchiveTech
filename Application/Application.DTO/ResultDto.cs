using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ResultDto
    {
        public bool Succeeded { get; init; }

        public string[] Errors { get; init; }
        public object? Data { get; set; }
        internal ResultDto(bool succeeded, IEnumerable<string> errors, object? data = null)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
            Data = data;
        }

        public static ResultDto Success(object? obj)
        {
            return new ResultDto(true, Array.Empty<string>(), obj);
        }

        public static ResultDto Failure(IEnumerable<string> errors)
        {                                                                                                                                                           
            return new ResultDto(false, errors);
        }                                       
    }
}
