using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaxCalculator.Database.Contexts;
using TaxCalculator.Database.Entities;

namespace TaxCalculator.Tests.SeedData;
public class SwedenGothenburgSeedData : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    public T GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    private IServiceProvider _serviceProvider;
    public async Task InitializeAsync()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<TaxCalculatorContext>(options =>
                            options.UseInMemoryDatabase(nameof(SwedenGothenburgSeedData)));
        serviceCollection.AddScoped<TaxCalculatorLogic>();
        _serviceProvider = serviceCollection.BuildServiceProvider();
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<TaxCalculatorContext>();
        using var stream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TaxCalculator.Tests.ExternalResources.SwedenGothenburg.json"));
        var read = await stream.ReadToEndAsync();
        var city = JsonSerializer.Deserialize<CityEntity>(read, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.Preserve
        });
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();
    }
}
