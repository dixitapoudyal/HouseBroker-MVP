using System.ComponentModel.DataAnnotations;

namespace HouseBroker.App.Properties.Dtos;

public class UpdatePropertyDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string PropertyType { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public double Area { get; set; }

    public int RoomCount { get; set; }

    public bool IsActive { get; set; } = true;
}