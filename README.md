# KotKas – Challenge 1 AI.NET

**Team:** Rémy Guns en Rik Van den Brande

KotKas is een .NET MAUI-app waarmee studenten op kot de kosten van gezamenlijke boodschappen eerlijk verdelen.
Je scant een kasticket. Azure AI leest de winkel, de datum en het totaal uit, en de app houdt het saldo per kotgenoot bij.

## Structuur

```
KotKasApi/                  .NET Web API (Layered Architecture + Repository Pattern)
  KotKas.Core/              Entities, Interfaces, Services (BalanceService)
  KotKas.Infrastructure/    KotKasContext (EF Core, SQL Server), Repositories
  KotKas.Api/               Controllers, DTOs
KotKasMaui/
  KotKas/                   .NET MAUI-app (MVVM Community Toolkit, lege code-behind)
    Models/ Views/ ViewModels/ Services/ Messages/ Helpers/
```

## API starten

1. Open `KotKasApi/KotKasApi.sln` en start het profiel **http** (http://localhost:5080).
2. De database `KotKasDb` wordt automatisch aangemaakt op `(localdb)\mssqllocaldb`, met testdata.
3. Ngrok voor Android: `ngrok http 5080` en vul de URL in bij `NgrokApiUrl` in `KotKas/Helpers/Constants.cs`.

## Azure AI

De sleutels staan in `KotKas/Helpers/Constants.cs`:

- **Computer Vision (OCR):** resource `kotkas` (KotKas-RG)
- **Custom Vision (classificatie):** project `KotKas Winkels`, gepubliceerd als `KotKasStoreModel` op `rgbigfive-Prediction`

### Custom Vision-model trainen (KotKasTrainer)

`KotKasTrainer/KotKas.Trainer` is een console-app die het model traint via de Custom Vision REST API:
project aanmaken, tags aanmaken, afbeeldingen uploaden uit `Images/<Tag>/`, trainen en publiceren.
De sleutels leest hij uit `kotkas-keys.json` (niet mee in de zip).

Huidige tags: `Colruyt`, `Lidl` en `Negative` (tickets van andere winkels).
We hadden te weinig eigen tickets per winkel. Daarom vulden we de Colruyt- en Lidl-foto's aan met gedraaide,
bijgesneden en lichtere/donkerdere varianten. Extra foto's toevoegen: zet ze in `Images/<Winkel>/` en start de trainer opnieuw.

Wordt de winkel niet herkend (of is de zekerheid lager dan 60%), dan zoekt de `ReceiptParser` de winkelnaam in de OCR-tekst.
Het scannen van totaal en datum werkt dus altijd.

## Werking van een scan

1. `CustomVisionService` herkent de winkel aan de foto.
2. `ComputerVisionService` leest alle tekstregels (OCR).
3. `ReceiptParser` zoekt het totaal, de datum en de winkel met de regels van die winkel (`StoreRules`).
4. Op de `ReviewExpensePage` controleert de gebruiker de gegevens en slaat hij de uitgave op.

## Navigatie en messages

`INavigationService` / `NavigationService` (pagina's via de serviceprovider), met deze messages:

- `ReceiptScannedMessage`: ScanViewModel → ReviewExpenseViewModel
- `ExpenseSelectedMessage`: ExpensesViewModel → ReviewExpenseViewModel
- `RoommateSelectedMessage`: RoommatesViewModel → RoommateDetailViewModel
- `RefreshExpensesMessage` / `RefreshRoommatesMessage`: lijsten en saldo's vernieuwen na opslaan of verwijderen
