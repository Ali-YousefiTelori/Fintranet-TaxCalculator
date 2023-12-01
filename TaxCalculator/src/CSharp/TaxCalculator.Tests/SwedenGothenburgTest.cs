using TaxCalculator.Database.Contexts;
using TaxCalculator.Tests.SeedData;

namespace TaxCalculator.Tests;
public class SwedenGothenburgTest : SwedenBaseTest, IClassFixture<SwedenGothenburgSeedData>
{
    public SwedenGothenburgTest(SwedenGothenburgSeedData fixture)
    {
        _taxCalculatorLogic = fixture.GetService<TaxCalculatorLogic>();
        _taxCalculatorContext = fixture.GetService<TaxCalculatorContext>();
    }

    protected override string CityName
    {
        get
        {
            return "Gothenburg";
        }
    }
}
