namespace KotKas.Models;

// Elke winkel bouwt zijn ticket anders op: per winkel andere woorden voor het totaal
public class StoreRules
{
    public Store Store { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string[] TotalKeywords { get; init; } = Array.Empty<string>();
    public string[] IgnoreKeywords { get; init; } = Array.Empty<string>();

    private static readonly string[] _commonIgnore =
        { "SUBTOTAAL", "SUB-TOTAAL", "SUBTOTAL", "BTW", "TVA", "KORTING", "REMISE", "SPAAR", "PUNTEN", "TERUG", "WISSELGELD", "RENDU" };

    public static StoreRules For(Store store) => store switch
    {
        Store.Colruyt => new StoreRules
        {
            Store = Store.Colruyt,
            DisplayName = "Colruyt",
            TotalKeywords = new[] { "TE BETALEN", "TOTAAL", "A PAYER", "TOTAL" },
            IgnoreKeywords = _commonIgnore
        },
        Store.Delhaize => new StoreRules
        {
            Store = Store.Delhaize,
            DisplayName = "Delhaize",
            TotalKeywords = new[] { "TOTAAL", "TOTAL", "A PAYER", "TE BETALEN" },
            IgnoreKeywords = _commonIgnore
        },
        Store.Aldi => new StoreRules
        {
            Store = Store.Aldi,
            DisplayName = "Aldi",
            TotalKeywords = new[] { "TOTAAL", "SUMME", "TOTAL", "TE BETALEN" },
            IgnoreKeywords = _commonIgnore
        },
        Store.Lidl => new StoreRules
        {
            Store = Store.Lidl,
            DisplayName = "Lidl",
            TotalKeywords = new[] { "TE BETALEN", "TOTAAL", "A PAYER", "TOTAL" },
            IgnoreKeywords = _commonIgnore
        },
        _ => new StoreRules
        {
            Store = Store.Other,
            DisplayName = string.Empty,
            TotalKeywords = new[] { "TE BETALEN", "TOTAAL", "TOTAL", "A PAYER", "SUMME", "BEDRAG", "AMOUNT" },
            IgnoreKeywords = _commonIgnore
        }
    };
}
