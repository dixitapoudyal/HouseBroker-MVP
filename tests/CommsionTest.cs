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
}