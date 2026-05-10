
namespace HouseBroker.App.Properties.Dtos;

public class PropertySearchDto
{
    public string? Location { get; set; }
    public string? PropertyType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}