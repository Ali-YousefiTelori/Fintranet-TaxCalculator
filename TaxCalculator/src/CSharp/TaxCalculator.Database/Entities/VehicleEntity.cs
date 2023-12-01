using System.Collections.Generic;

namespace TaxCalculator.Database.Entities
{
    public class VehicleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CityVehicleTaxExemptionEntity> CityVehicleTaxExemptions { get; set; }
    }
}
