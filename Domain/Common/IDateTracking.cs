using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public interface IDateTracking
    {
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset LastUpdateDate { get; set; }
    }
}
