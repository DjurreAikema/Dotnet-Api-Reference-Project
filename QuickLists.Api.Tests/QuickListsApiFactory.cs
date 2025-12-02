using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using QuickLists.Infrastructure.Data;

namespace QuickLists.Api.Tests;

public class QuickListsApiFactory : WebApplicationFactory<Program>
{
    private static readonly string TestDbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Use the SAME database name for all requests in a test class
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(TestDbName));
        });
    }
}