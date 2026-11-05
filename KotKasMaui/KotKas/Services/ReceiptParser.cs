using System.Globalization;
using System.Text.RegularExpressions;
using KotKas.Models;

namespace KotKas.Services;

// Een ticket heeft telkens een ander aantal producten, dus het totaal staat nooit op dezelfde plaats.
// Daarom zoeken we in de tekst naar de regel met het totaal, de datum en de bedragen.
public partial class ReceiptParser : IReceiptParser
{
    [GeneratedRegex(@"(?<!\d)(\d{1,4})\s?[.,]\s?(\d{2})(?!\d)")]
    private static partial Regex AmountRegex();

    [GeneratedRegex(@"(?<!\d)(\d{1,2})[/\-.](\d{1,2})[/\-.](\d{4}|\d{2})(?!\d)")]
    private static partial Regex DateRegex();

    public ScannedReceipt Parse(List<string> lines, Store store)
    {
        // Werd de winkel niet herkend door Custom Vision? Kijk dan of de naam op het ticket staat
        if (store == Store.Other)
            store = DetectStoreFromText(lines);

        var rules = StoreRules.For(store);
        var receipt = new ScannedReceipt
        {
            DetectedStore = store,
            Lines = lines,
            StoreName = string.IsNullOrEmpty(rules.DisplayName) ? GuessStoreName(lines) : rules.DisplayName
        };

        var total = FindTotal(lines, rules);
        if (total.HasValue)
        {
            receipt.TotalAmount = total.Value;
            receipt.TotalFound = true;
        }

        var date = FindDate(lines);
        if (date.HasValue)
        {
            receipt.PurchaseDate = date.Value;
            receipt.DateFound = true;
        }

        return receipt;
    }

    private static Store DetectStoreFromText(List<string> lines)
    {
        var text = string.Join(" ", lines).ToUpperInvariant();
        foreach (var store in new[] { Store.Colruyt, Store.Delhaize, Store.Aldi, Store.Lidl })
        {
            if (text.Contains(store.ToString().ToUpperInvariant()))
                return store;
        }
        return Store.Other;
    }

    private static string GuessStoreName(List<string> lines)
    {
        // Bij een onbekende winkel staat de naam meestal bovenaan het ticket
        var first = lines.FirstOrDefault(l => l.Count(char.IsLetter) >= 3);
        return first?.Trim() ?? string.Empty;
    }

    private static decimal? FindTotal(List<string> lines, StoreRules rules)
    {
        // Per winkel andere woorden, in volgorde van belangrijkheid
        foreach (var keyword in rules.TotalKeywords)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var upper = lines[i].ToUpperInvariant();
                if (!upper.Contains(keyword) || rules.IgnoreKeywords.Any(upper.Contains))
                    continue;

                // Het bedrag staat op dezelfde regel of (door de OCR) op één van de volgende regels
                for (int j = i; j < Math.Min(i + 3, lines.Count); j++)
                {
                    var amount = LastAmount(lines[j]);
                    if (amount.HasValue && amount.Value > 0)
                        return amount;
                }
            }
        }

        // Geen totaal-regel gevonden: het grootste bedrag op het ticket is meestal het totaal
        var amounts = lines
            .Where(l => !rules.IgnoreKeywords.Any(l.ToUpperInvariant().Contains))
            .Select(LastAmount)
            .Where(a => a.HasValue)
            .Select(a => a!.Value)
            .ToList();

        return amounts.Count > 0 ? amounts.Max() : null;
    }

    private static decimal? LastAmount(string line)
    {
        var matches = AmountRegex().Matches(line);
        if (matches.Count == 0)
            return null;

        var match = matches[^1];
        var value = $"{match.Groups[1].Value}.{match.Groups[2].Value}";
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)
            ? amount
            : null;
    }

    private static DateTime? FindDate(List<string> lines)
    {
        foreach (var line in lines)
        {
            foreach (Match match in DateRegex().Matches(line))
            {
                int day = int.Parse(match.Groups[1].Value);
                int month = int.Parse(match.Groups[2].Value);
                int year = int.Parse(match.Groups[3].Value);
                if (year < 100)
                    year += 2000;

                if (month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(Math.Clamp(year, 1, 9999), month))
                    continue;

                var date = new DateTime(year, month, day);
                if (date <= DateTime.Today.AddDays(1) && date.Year >= 2000)
                    return date;
            }
        }
        return null;
    }
}
