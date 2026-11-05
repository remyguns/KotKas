namespace KotKas.Services;

public interface IComputerVisionService
{
    // OCR: alle tekst van het kasticket, regel per regel
    Task<List<string>> ReadLinesAsync(Stream imageStream);
}
