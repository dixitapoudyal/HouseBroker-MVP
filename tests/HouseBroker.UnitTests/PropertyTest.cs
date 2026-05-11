using FluentAssertions;
using HouseBroker.App.Auth.Interfaces;
using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Properties.Mappings;
using HouseBroker.Infra.Properties;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using HouseBroker.App.Properties.Dtos;

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

		// assert
		// the result dto should look right
		result.Id.Should().BeGreaterThan(0);
		result.Title.Should().Be("3BHK in Lalitpur");
		result.CommissionAmount.Should().Be(100000m);

		// and the property should actually be in the database
		var saved = await db.Properties.Include(p => p.Images).FirstAsync();
		saved.BrokerId.Should().Be("xyz123");
		saved.Images.Should().HaveCount(1);
	}

	[Fact]
	public async Task GetProeprty_dont_show_commission_to_non_owner()
	{
		var db = TestHelper.CreateInMemoryDb();
		var mapper = new PropertyMapper();
		var commissionMock = new Mock<ICommissionService>();
		var service = new PropertyService(db, mapper, commissionMock.Object);

		// broker A creates a property
		var dto = new CreatePropertyDto
		{
			Title = "Test11",
			Description = "x",
			PropertyType = "Land",
			Location = "Kathmandu",
			Price = 3000000,
			ContactPhone = "9899554645"
		};

		var created = await service.CreateAsync(dto, brokerId: "broker1");

		var resultForOtherUser = await service.GetByIdAsync(created.Id, currentUserId: "broker2");//another user requests property

		// commission should not be set since broker2 doesn't own that listing
		resultForOtherUser.CommissionAmount.Should().BeNull();

		// same call with the owner should return commission
		commissionMock.Setup(c => c.CalculateAsync(3000000m)).ReturnsAsync(60_000m);
		var resultForOwner = await service.GetByIdAsync(created.Id, currentUserId: "broker1");
		resultForOwner.CommissionAmount.Should().Be(60000m);
	}
	[Fact]
	public async Task UpdateAsync_should_throw_when_called_by_non_owner()
	{
		var db = TestHelper.CreateInMemoryDb();
		var mapper = new PropertyMapper();
		var commissionMock = new Mock<ICommissionService>();
		commissionMock.Setup(c => c.CalculateAsync(It.IsAny<decimal>()))
					  .ReturnsAsync(0m);

		var service = new PropertyService(db, mapper, commissionMock.Object);

		
		var createDto = new CreatePropertyDto
		{
			Title = "TestProperty",
			Description = "owned by B1",
			PropertyType = "Mall",
			Location = "Kathmandu",
			Price = 10000000,
			ContactPhone = "985738753"
		};
		var created = await service.CreateAsync(createDto, brokerId: "broker1");

		var updateDto = new UpdatePropertyDto
		{
			Title = "TestPropety",
			Description = "broker 2 trying",
			PropertyType = "Mall",
			Location = "Kathmandu",
			Price = 5_000_000,
			IsActive = true
		};

		// act
		// broker2 tries to update broker 1's property
		Func<Task> hijackAttempt = async () =>
			await service.UpdateAsync(created.Id, updateDto, brokerId: "broker2");

		await hijackAttempt.Should().ThrowAsync<UnauthorizedAccessException>();//assert that it throws unauthorized access exception
	}

	[Fact]
	public async Task DeleteAsync_should_throw_when_called_by_non_owner()
	{
		var db = TestHelper.CreateInMemoryDb();
		var mapper = new PropertyMapper();
		var commissionMock = new Mock<ICommissionService>();
		var service = new PropertyService(db, mapper, commissionMock.Object);

		var createDto = new CreatePropertyDto
		{
			Title = "TestDelete",
			Description = "owned by B1",
			PropertyType = "Apartment",
			Location = "Lalitpur",
			Price = 3000000,
			ContactPhone = "69846378456"
		};
		var created = await service.CreateAsync(createDto, brokerId: "broker-1");

		// broker 2 tries to delete broker1's property
		Func<Task> deleteAttempt = async () =>
			await service.DeleteAsync(created.Id, brokerId: "broker-2");

		// ---- assert ----
		await deleteAttempt.Should().ThrowAsync<UnauthorizedAccessException>();

		// sanity test - the property should still be in the db since the delete should have failed
		var stillThere = await db.Properties.FindAsync(created.Id);
		stillThere.Should().NotBeNull();
	}
}