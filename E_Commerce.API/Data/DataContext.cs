using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
