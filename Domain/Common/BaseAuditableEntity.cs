using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class BaseAuditableEntity<T>:BaseEntity<T>, IDateTracking
    {
        public T CreatedBy { get; set; }
        public T LastUpdateBy { get; set; }
        public DateTimeOffset CreatedDate { get ; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        
    }
}
