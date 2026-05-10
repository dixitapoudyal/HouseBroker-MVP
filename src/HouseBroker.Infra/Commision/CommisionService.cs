using HouseBroker.App.Auth.Interfaces;
using HouseBroker.Domain.Entities;
using HouseBroker.Infra.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HouseBroker.Infra.Commission;

public class CommissionService : ICommissionService
{
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public CommissionService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<decimal> CalculateAsync(decimal price)
    {
        var rates = await GetRatesAsync();

        // finds tier where MinPrice <= price < MaxPrice (if null , no upper bound)
        var rate = rates.FirstOrDefault(r =>
            r.MinPrice <= price && (r.MaxPrice == null || price < r.MaxPrice));

        if (rate is null)
            return 0m;

        return price * rate.Rate;
    }

    private async Task<List<CommissionPrice>> GetRatesAsync()
    {
        if (_cache.TryGetValue<List<CommissionPrice>>("commission_rates", out var cached) && cached is not null)
            return cached;

        var rates = await _db.CommissionRates
            .OrderBy(r => r.MinPrice)
            .ToListAsync();

        _cache.Set("commission_rates", rates, TimeSpan.FromMinutes(20));//caches tiers for 20 minutes
        return rates;
    }
}