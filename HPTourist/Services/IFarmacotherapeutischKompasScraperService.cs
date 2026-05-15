namespace HPTourist.Services;

public interface IFarmacotherapeutischKompasScraperService
{
    Task<string?> GetContraIndicationAsync(string activeSubstance);
}
