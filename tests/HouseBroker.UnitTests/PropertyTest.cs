using FluentAssertions;
using HouseBroker.App.Commission.Interfaces;
using HouseBroker.App.Properties.Dtos;
using HouseBroker.App.Properties.Mappings;
using HouseBroker.Domain.Enums;
using HouseBroker.Infra.Properties;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HouseBroker.UnitTests.Properties;

public class PropertyTest
{
	[Fact]
	public async Task CreateAsync_saves_property_and_broker_id()
	{
		var db = TestHelper.CreateInMemoryDb();// fresh in-memory db

		var mapper = new PropertyMapper();

		var commissionMock = new Mock<ICommissionService>();
		commissionMock.Setup(c => c.CalculateAsync(It.IsAny<decimal>()))
					  .ReturnsAsync(100_000m);

		var service = new PropertyService(db, mapper, commissionMock.Object);

		// build the input dto
		var dto = new CreatePropertyDto
		{
			Title = "3BHK in Lalitpur",
			Description = "ebside mainroad",
			PropertyType = "House",
			Location = "Bkt",
			Price = 5000000,
			RoomCount = 5,
			AreaSqFt = 1100,
			ContactPhone = "+977673863",
			ImageUrls = new List<string> { "https://example.com/1.jpg" }
		};

		var result = await service.CreateAsync(dto, brokerId: "xyz123");

		// ---- assert ----
		// the result dto should look right
		result.Id.Should().BeGreaterThan(0);
		result.Title.Should().Be("3BHK in Lalitpur");
		result.CommissionAmount.Should().Be(100000m);

		// and the property should actually be in the database
		var saved = await db.Properties.Include(p => p.Images).FirstAsync();
		saved.BrokerId.Should().Be("xyz123");
		saved.Images.Should().HaveCount(1);
	}
}