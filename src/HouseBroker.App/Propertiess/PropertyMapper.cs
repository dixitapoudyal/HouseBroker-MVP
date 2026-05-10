using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Properties.Dtos;
using HouseBroker.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace HouseBroker.App.Properties.Mappings;

[Mapper]
public partial class PropertyMapper
{
    [MapperIgnoreTarget(nameof(PropertyListing.Id))]
    [MapperIgnoreTarget(nameof(PropertyListing.BrokerId))]
    [MapperIgnoreTarget(nameof(PropertyListing.Images))]
    [MapperIgnoreTarget(nameof(PropertyListing.CreatedOn))]
    [MapperIgnoreTarget(nameof(PropertyListing.IsActive))]
    public partial PropertyListing ToEntity(CreatePropertyDto dto);

    [MapperIgnoreTarget(nameof(PropertyListing.BrokerId))]
    [MapperIgnoreTarget(nameof(PropertyListing.Images))]
    [MapperIgnoreTarget(nameof(PropertyListing.CreatedOn))]
    [MapperIgnoreTarget(nameof(PropertyListing.UpdatedOn))]
    [MapperIgnoreTarget(nameof(PropertyListing.IsActive))]
    public partial void UpdateEntity(UpdatePropertyDto dto, PropertyListing property);

    public PropertyDto ToDto(PropertyListing property) => new()
    {
        Id = property.Id,
        Title = property.Title,
        Description = property.Description,
        PropertyType = property.PropertyType,
        Location = property.Location,
        Price = property.Price,
        Area = property.Area,
        RoomCount = property.RoomCount,
        IsActive = property.IsActive,
        CreatedOn = property.CreatedOn,
        UpdatedOn = property.UpdatedOn,
        ImageUrls = property.Images?.Select(i => i.ImageUrl).ToList() ?? new()
    };
}