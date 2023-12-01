using Microsoft.EntityFrameworkCore;
using TaxCalculator.Database.Contexts;

namespace TaxCalculator.Tests;

public abstract class SwedenBaseTest
{
    protected TaxCalculatorLogic _taxCalculatorLogic;
    protected TaxCalculatorContext _taxCalculatorContext;

    protected abstract string CityName { get; }
    [Theory]
    [InlineData("Car", new string[] {  "2013-01-14 21:00:00","2013-01-15 21:00:00","2013-02-07 06:23:27","2013-02-07 15:27:00",
                                                         "2013-02-08 06:27:00","2013-02-08 06:20:27","2013-02-08 14:35:00","2013-02-08 15:29:00",
                                                         "2013-02-08 15:47:00","2013-02-08 16:01:00","2013-02-08 16:48:00","2013-02-08 17:49:00",
                                                         "2013-02-08 18:29:00","2013-02-08 18:35:00","2013-03-26 14:25:00","2013-03-28 14:07:27" })]
    public async Task VehicleTaxCalculate(string vehicleName, string[] dates)
    {
        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, dates.Select(DateTime.Parse).ToArray());

        Assert.Equal(0, calculate[DateOnly.Parse("2013-01-14")]);
        Assert.Equal(0, calculate[DateOnly.Parse("2013-01-15")]);
        Assert.Equal(21, calculate[DateOnly.Parse("2013-02-07")]);
        Assert.Equal(60, calculate[DateOnly.Parse("2013-02-08")]);
        Assert.Equal(8, calculate[DateOnly.Parse("2013-03-26")]);
        Assert.Equal(8, calculate[DateOnly.Parse("2013-03-28")]);
    }

    [Theory]
    [InlineData("Tractor", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Emergency", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Busses", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Motorcycles", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Diplomat", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Military", new string[] { "2013-03-28 14:07:27" })]
    [InlineData("Foreign", new string[] { "2013-03-28 14:07:27" })]
    public async Task VehicleTaxExemptionTest(string vehicleName, string[] dates)
    {
        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, dates.Select(DateTime.Parse).ToArray());

        Assert.True(calculate.All(x => x.Value == 0));
    }

    [Theory]
    [InlineData("Car")]
    public async Task JulyTest(string vehicleName)
    {
        List<DateTime> daysInJuly = new List<DateTime>();

        int year = 2013;
        int month = 7;

        for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
        {
            DateTime currentDate = new DateTime(year, month, day);
            daysInJuly.Add(currentDate);
        }

        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, daysInJuly.ToArray());
        Assert.True(calculate.All(x => x.Value == 0));
    }

    [Theory]
    [InlineData("Car")]
    public async Task HolidaysTest(string vehicleName)
    {
        var holidays = await _taxCalculatorContext.PublicHolidays.ToListAsync();
        List<DateTime> someDates = new List<DateTime>();
        for (int i = 0; i < 20; i++)
        {
            foreach (var item in holidays)
            {
                someDates.Add(item.Date.ToDateTime(new TimeOnly().AddHours(i)));
            }
        }

        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, someDates.ToArray());
        Assert.True(calculate.All(x => x.Value == 0));
    }

    [Theory]
    [InlineData("Car")]
    public async Task DaysBeforeHolidaysTest(string vehicleName)
    {
        var holidays = await _taxCalculatorContext.PublicHolidays.ToListAsync();
        List<DateTime> someDates = new List<DateTime>();
        for (int i = 0; i < 20; i++)
        {
            foreach (var item in holidays)
            {
                someDates.Add(item.Date.ToDateTime(new TimeOnly().AddHours(i)).AddDays(-1));
            }
        }

        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, someDates.ToArray());
        Assert.True(calculate.All(x => x.Value == 0));
    }

    [Theory]
    [InlineData("Car")]
    public async Task WeekendTest(string vehicleName)
    {
        List<DateOnly> weekendsInYear = new List<DateOnly>();

        int year = 2013;

        for (int month = 1; month <= 12; month++)
        {
            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                DateOnly currentDate = new DateOnly(year, month, day);

                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendsInYear.Add(currentDate);
                }
            }
        }

        List<DateTime> someDates = new List<DateTime>();
        for (int i = 0; i < 20; i++)
        {
            foreach (var item in weekendsInYear)
            {
                someDates.Add(item.ToDateTime(new TimeOnly().AddHours(i)));
            }
        }

        var calculate = await _taxCalculatorLogic.Calculate(CityName, vehicleName, someDates.ToArray());
        Assert.True(calculate.All(x => x.Value == 0));
    }
}
