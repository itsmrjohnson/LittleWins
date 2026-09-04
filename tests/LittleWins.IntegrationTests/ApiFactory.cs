using LittleWins.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LittleWins.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly IntegrationTestFixture _fixture;

    public ApiFactory(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                service =>
                    service.ServiceType ==
                    typeof(DbContextOptions<LittleWinsDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<LittleWinsDbContext>(
                options =>
                    options.UseSqlServer(
                        _fixture.ConnectionString));
        });
    }
}