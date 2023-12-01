namespace TaxCalculator.Database.Entities;
public class CityVehicleTaxExemptionEntity
{
    public int CityId { get; set; }
    public int VehicleId { get; set; }

    public CityEntity City { get; set; }
    public VehicleEntity Vehicle { get; set; }
}
