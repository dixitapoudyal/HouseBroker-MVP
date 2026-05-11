using HouseBroker.Infra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HouseBroker.UnitTests;

public static class TestHelper
{
	public static ApplicationDbContext CreateInMemoryDb()
	{
		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())   // fresh DB instance on each test
			.Options;
		return new ApplicationDbContext(options);
	}
}