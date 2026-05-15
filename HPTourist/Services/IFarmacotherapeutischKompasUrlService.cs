using System;

namespace HPTourist.Services;

public interface IFarmacotherapeutischKompasUrlService
{
    string CreatePreparationtextUrl(string activeSubstance);
}
