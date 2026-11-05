using KotKas.Models;

namespace KotKas.Services;

public interface ICustomVisionService
{
    // Eigen getraind model: van welke winkel komt het kasticket?
    Task<(Store Store, double Probability)> ClassifyStoreAsync(Stream imageStream);
}
