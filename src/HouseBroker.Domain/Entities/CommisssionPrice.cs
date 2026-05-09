using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Domain.Entities
{
    public class CommissionRate : BaseEntity
    {
        public decimal MinPrice { get; set; }
        public decimal? MaxPrice { get; set; } 
        public decimal Rate { get; set; }        // say 0.01 = 1%
    }
}
