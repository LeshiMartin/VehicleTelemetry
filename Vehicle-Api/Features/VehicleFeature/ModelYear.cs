namespace Vehicle_Api.Features.VehicleFeature;

public readonly record struct ModelYear
{
    public ModelYear(int Year)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Year, 2300);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(Year, 1900);
        this.Year = Year;
    }

    public int Year { get; init; }
    public static implicit operator ModelYear(int year) => new(year);
    public static implicit operator int(ModelYear modelYear) => modelYear.Year;

    public void Deconstruct(out int year)
    {
        year = this.Year;
    }
}