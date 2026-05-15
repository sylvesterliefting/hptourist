using HtmlAgilityPack;

namespace HPTourist.Services;

public sealed class FarmacotherapeutischKompasScraperService
    : IFarmacotherapeutischKompasScraperService
{
    private readonly HttpClient _httpClient;
    private readonly IFarmacotherapeutischKompasUrlService _urlService;

    public FarmacotherapeutischKompasScraperService(
        HttpClient httpClient,
        IFarmacotherapeutischKompasUrlService urlService)
    {
        _httpClient = httpClient;
        _urlService = urlService;
    }

    public async Task<string?> GetContraIndicationAsync(string activeSubstance)
    {
        var url = _urlService.CreatePreparationtextUrl(activeSubstance);
        var html = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var heading = doc.DocumentNode
            .Descendants()
            .FirstOrDefault(x =>
                x.InnerText.Trim()
                    .Equals("Contra-indicaties", StringComparison.OrdinalIgnoreCase));

        if (heading is null)
            return null;

        var content = heading
            .ParentNode
            .InnerText
            .Trim();

        return HtmlEntity.DeEntitize(content);
    }
}