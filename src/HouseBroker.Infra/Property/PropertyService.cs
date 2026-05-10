using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Auth.Interfaces;
using HouseBroker.App.Properties.Dtos;
using HouseBroker.App.Properties.Interfaces;
using HouseBroker.App.Properties.Mappings;
using HouseBroker.Domain.Entities;
using HouseBroker.Infra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.Infra.Properties;

public class PropertyService : IPropertyService
{
    private readonly ApplicationDbContext _db;
    private readonly PropertyMapper _mapper;
    private readonly ICommissionService _commission;

    public PropertyService(
        ApplicationDbContext db,
        PropertyMapper mapper,
        ICommissionService commission)
    {
        _db = db;
        _mapper = mapper;
        _commission = commission;
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyDto dto, string brokerId)
    {
        var property = _mapper.ToEntity(dto);
        property.BrokerId = brokerId;
        property.IsActive = true;
        property.Images = dto.ImageUrls
            .Select((url, idx) => new PropertyImage { ImageUrl = url })
            .ToList();

        _db.Properties.Add(property);
        await _db.SaveChangesAsync();

        var result = _mapper.ToDto(property);
        result.CommissionAmount = await _commission.CalculateAsync(property.Price);
        return result;
    }

    public async Task<PropertyDto> UpdateAsync(int id, UpdatePropertyDto dto, string brokerId)
    {
        var property = await _db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Property {id} not found.");

        if (property.BrokerId != brokerId)
            throw new UnauthorizedAccessException("You don't own this listing.");

        _mapper.UpdateEntity(dto, property);
        property.UpdatedOn = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var result = _mapper.ToDto(property);
        result.CommissionAmount = await _commission.CalculateAsync(property.Price);
        return result;
    }

    public async Task DeleteAsync(int id, string brokerId)
    {
        var property = await _db.Properties.FindAsync(id)
            ?? throw new KeyNotFoundException($"Property {id} not found.");

        if (property.BrokerId != brokerId)
            throw new UnauthorizedAccessException("You don't own this listing.");

        _db.Properties.Remove(property);
        await _db.SaveChangesAsync();
    }

    public async Task<PropertyDto> GetByIdAsync(int id, string? currentUserId)
    {
        var property = await _db.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Property {id} not found.");

        var dto = _mapper.ToDto(property);

        // commission only visible to the owner
        if (!string.IsNullOrEmpty(currentUserId) && property.BrokerId == currentUserId)
            dto.CommissionAmount = await _commission.CalculateAsync(property.Price);

        return dto;
    }

    public async Task<List<PropertyDto>> SearchAsync(PropertySearchDto filter, string? currentUserId)
    {
        var query = _db.Properties
            .Include(p => p.Images)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(p => p.Location.Contains(filter.Location));

        if (!string.IsNullOrWhiteSpace(filter.PropertyType))
            query = query.Where(p => p.PropertyType == filter.PropertyType);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        var properties = await query
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();

        var results = new List<PropertyDto>();
        foreach (var property in properties)
        {
            var dto = _mapper.ToDto(property);
            if (!string.IsNullOrEmpty(currentUserId) && property.BrokerId == currentUserId)
                dto.CommissionAmount = await _commission.CalculateAsync(property.Price);
            results.Add(dto);
        }
        return results;
    }
}