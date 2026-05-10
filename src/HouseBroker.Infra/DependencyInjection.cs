using HouseBroker.App.Auth.Interfaces;
using HouseBroker.App.Properties.Interfaces;
using HouseBroker.App.Properties.Mappings;
using HouseBroker.Infra.Commission;
using HouseBroker.Infra.DBContext;
using HouseBroker.Infra.Identity;
using HouseBroker.Infra.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HouseBroker.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false; //for demo
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            services.Configure<JWTConfiguration>(configuration.GetSection("Jwt"));
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<PropertyMapper>();
            services.AddMemoryCache();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<IPropertyService, PropertyService>();

            return services;
        }
    }
}
