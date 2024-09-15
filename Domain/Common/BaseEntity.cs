using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public class BaseEntity<T>:IBaseEntity<T>,ISoftDelete
    {
        public T Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
