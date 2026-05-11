using FluentAssertions;
using HouseBroker.Domain.Entities;
using HouseBroker.Infra.Commission;
using HouseBroker.Infra.DBContext;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace HouseBroker.UnitTests.Commission;

public class CommissionTest
{
	[Fact]
	public async Task CalculateAsync_returns_zero_when_no_matching_tier()
	{
		var db = TestHelper.CreateInMemoryDb();
		var cache = new MemoryCache(new MemoryCacheOptions());
		var sut = new CommissionService(db, cache);

		var result = await sut.CalculateAsync(1_000_000);

		result.Should().Be(0);
	}
	private static CommissionService CreateSut()
	{
		var db = TestHelper.CreateInMemoryDb();
		db.CommissionRates.AddRange(
			new CommissionRate { Id = 1, MinPrice = 0, MaxPrice = 5000000m, Rate = 0.0200m, IsActive = true },
			new CommissionRate { Id = 2, MinPrice = 5000000m, MaxPrice = 10_000_000m, Rate = 0.0175m, IsActive = true },
			new CommissionRate { Id = 3, MinPrice = 10000000m, MaxPrice = null, Rate = 0.0150m, IsActive = true }
		);
		db.SaveChanges();
		var cache = new MemoryCache(new MemoryCacheOptions());
		return new CommissionService(db, cache);
	}
	[Theory]

	[InlineData(1000000, 20000)]
	[InlineData(4999999, 99999.98)]
	// tier 2
	[InlineData(5000000, 87500)]
	[InlineData(7500000, 131250)]
	// tier 3
	[InlineData(10000000, 150000)]
	[InlineData(50000000, 750000)]
	public async Task CalculateAsync_commission(
	decimal price, decimal expected)
	{
		var sut = CreateSut();
		var result = await sut.CalculateAsync(price);
		result.Should().Be(expected);
	}
}