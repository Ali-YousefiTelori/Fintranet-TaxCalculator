using System;

namespace TaxCalculator.Database.Entities;
public class PublicHolidayEntity
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }

    /// <summary>
    /// Tax exemptions could have different rules for each city
    /// </summary>
    public int CityId { get; set; }
    public CityEntity City { get; set; }
}
