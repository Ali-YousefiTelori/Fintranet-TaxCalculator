using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TaxCalculator.Database.Contexts;
using TaxCalculator.Database.Entities;

namespace TaxCalculator;
public class TaxCalculatorLogic
{
    TaxCalculatorContext _taxCalculatorContext;
    public TaxCalculatorLogic(TaxCalculatorContext taxCalculatorContext)
    {
        _taxCalculatorContext = taxCalculatorContext;
    }

    public async Task<Dictionary<DateOnly, decimal>> Calculate(string cityName, string vehicleName, DateTime[] dates)
    {
        var city = _taxCalculatorContext.Cities.FirstOrDefault(x => x.Name == cityName);

        if (city is null)
            return new Dictionary<DateOnly, decimal>();
        if (_taxCalculatorContext.CityVehicleTaxExemptions.Any(x => x.CityId == city.Id && x.Vehicle.Name == vehicleName))
            return new Dictionary<DateOnly, decimal>();

        var minDate = DateOnly.FromDateTime(dates.Min());
        var maxDate = DateOnly.FromDateTime(dates.Max()).AddDays(1);

        Dictionary<DateOnly, decimal> result = new Dictionary<DateOnly, decimal>();

        var holidays = (await _taxCalculatorContext.PublicHolidays
            .Where(x => x.Date >= minDate && x.Date <= maxDate)
            .Select(x => x.Date)
            .ToListAsync()).ToImmutableHashSet();

        var taxUnits = await _taxCalculatorContext.TaxUnits.ToListAsync();

        foreach (var day in dates.GroupBy(x => DateOnly.FromDateTime(x)))
        {
            if (!CanCalculateForThisDate(day.Key, holidays))
                result.Add(day.Key, 0);
            else
                result.Add(day.Key, CalculateTaxForADay(day.Select(x => x.TimeOfDay), taxUnits));
        }
        return result;
    }

    decimal CalculateTaxForADay(IEnumerable<TimeSpan> tollTimesOfDay, List<TaxUnitEntity> taxUnits)
    {
        var sum = tollTimesOfDay
            .Select(timeOfDay => taxUnits.Where(x => IsTimeSpanInRange(x.FromTime, x.ToTime, timeOfDay))
            .Sum(x => x.Amount))
            .Sum();

        if (sum > 60)
            return 60;
        return sum;
    }

    public bool IsTimeSpanInRange(TimeSpan from, TimeSpan to, TimeSpan target)
    {
        return (target >= from && target <= to);
    }

    /// <summary>
    /// The tax is not charged on weekends (Saturdays and Sundays),
    /// public holidays, days before a public holiday and during the month of July.
    /// </summary>
    /// <returns></returns>
    bool CanCalculateForThisDate(DateOnly dateOnly, ImmutableHashSet<DateOnly> holidays)
    {
        if (dateOnly.DayOfWeek == DayOfWeek.Saturday || dateOnly.DayOfWeek == DayOfWeek.Sunday)
            return false;
        else if (holidays.Contains(dateOnly))
            return false;
        //days before a public holiday 
        else if (holidays.Contains(dateOnly.AddDays(1)))
            return false;
        //during the month of July
        else if (dateOnly.Month == 7)
            return false;

        return true;
    }
}
