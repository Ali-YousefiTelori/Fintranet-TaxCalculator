using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxCalculator.Database.Contexts;
using TaxCalculator.Tests.SeedData;

namespace TaxCalculator.Tests;
public class SwedenStockholmTest : SwedenBaseTest, IClassFixture<SwedenStockholmSeedData>
{
    public SwedenStockholmTest(SwedenStockholmSeedData fixture)
    {
        _taxCalculatorLogic = fixture.GetService<TaxCalculatorLogic>();
        _taxCalculatorContext = fixture.GetService<TaxCalculatorContext>();
    }

    protected override string CityName
    {
        get
        {
            return "Stockholm";
        }
    }
}
