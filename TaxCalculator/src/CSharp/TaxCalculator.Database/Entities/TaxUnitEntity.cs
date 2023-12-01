using System;
using TaxCalculator.DataTypes;

namespace TaxCalculator.Database.Entities;
public class TaxUnitEntity
{
    public int Id { get; set; }

    public TimeSpan FromTime { get; set; }
    public TimeSpan ToTime { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public  decimal Amount { get; set; }

    /// <summary>
    /// Taxes could have different rules for each city
    /// </summary>
    public int CityId { get; set; }
    public CityEntity City { get; set; }
}
