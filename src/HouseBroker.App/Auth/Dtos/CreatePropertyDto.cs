using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.App.Auth.Dtos
{
    public class CreatePropertyDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string PropertyType { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string ContactPhone { get; set; } = string.Empty;
        [Range(0, 100)]
        public int RoomCount { get; set; }

        [Range(0, double.MaxValue)]
        public double AreaSqFt { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
