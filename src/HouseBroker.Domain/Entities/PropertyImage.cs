using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Domain.Entities
{
    public class PropertyImage
    {
        public int PropertyId { get; set; }
        public Property? Property { get; set; }
        public string ImageUrl { get; set; }
    }
}
