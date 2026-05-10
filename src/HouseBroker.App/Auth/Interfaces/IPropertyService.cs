using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Properties.Dtos;

namespace HouseBroker.App.Properties.Interfaces;

public interface IPropertyService
{
    Task<PropertyDto> CreateAsync(CreatePropertyDto dto, string brokerId);
    Task<PropertyDto> UpdateAsync(int id, UpdatePropertyDto dto, string brokerId);
    Task DeleteAsync(int id, string brokerId);
    Task<PropertyDto> GetByIdAsync(int id, string? currentUserId);
    Task<List<PropertyDto>> SearchAsync(PropertySearchDto filter, string? currentUserId);
}