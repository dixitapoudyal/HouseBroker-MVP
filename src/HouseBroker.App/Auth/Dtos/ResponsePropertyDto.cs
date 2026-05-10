using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.App.Auth.Dtos
{
    public class ResponsePropertyDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public double Area { get; set; }
        public int RoomCount { get; set; }

        public string BrokerId { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<string> ImageUrls { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        // null unless current user is the owning broker
        public decimal? CommissionAmount { get; set; }
    }
}
