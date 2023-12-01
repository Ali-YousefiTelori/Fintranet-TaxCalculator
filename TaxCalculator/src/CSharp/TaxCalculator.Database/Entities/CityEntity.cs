using System.Collections.Generic;

namespace TaxCalculator.Database.Entities;
public class CityEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<TaxUnitEntity> TaxUnits { get; set; }
    public ICollection<PublicHolidayEntity> PublicHolidays { get; set; }
    public ICollection<CityVehicleTaxExemptionEntity> CityVehicleTaxExemptions { get; set; }
}
