using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Domain.Entities
{
    public class PropertyInfo : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public double Area { get; set; }
        public decimal Price { get; set; }
        public int RoomCount { get; set; }
        public string BrokerId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public PropertyImage Images { get; set; } = new PropertyImage();

    }
}
