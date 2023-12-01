using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using TaxCalculator.Database.Contexts;
using TaxCalculator.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaxCalculator.Tests.SeedData;
public class SwedenStockholmSeedData : IAsyncLifetime
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
                            options.UseInMemoryDatabase(nameof(SwedenStockholmSeedData)));
        serviceCollection.AddScoped<TaxCalculatorLogic>();
        _serviceProvider = serviceCollection.BuildServiceProvider();
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<TaxCalculatorContext>();
        using var stream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TaxCalculator.Tests.ExternalResources.SwedenStockholm.json"));
        var read = await stream.ReadToEndAsync();
        var city = JsonSerializer.Deserialize<CityEntity>(read, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.Preserve
        });
        await dbContext.Cities.AddAsync(city);
        await dbContext.SaveChangesAsync();
    }
}
