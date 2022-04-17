namespace JobApplicationLibrary.Interfaces
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);
        ICountryDataProvider CountryDataProvider { get; }
    }

    public interface ICountryData
    {
        string Country { get; }
    }

    public interface ICountryDataProvider
    {
        ICountryData CountryData { get; }
    }
}
