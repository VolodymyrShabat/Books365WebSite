using Books365WebSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Books365Website.Tests
{
    public class DbFixture
    {
        public DbFixture()
        {
            
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddDbContext<Context>(options => options.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = Books365; Trusted_Connection = True; MultipleActiveResultSets = true"),
                    ServiceLifetime.Transient);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
