using KotKas.Models;

namespace KotKas.Services;

public class RoommateDataService : ApiService<Roommate>, IRoommateDataService
{
    public RoommateDataService() : base("roommates")
    {
    }
}
