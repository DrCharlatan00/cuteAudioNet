using cuteAudioNet.Postgresql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace cuteAudioNet.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(src => {
                var old = src.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PgContext>));

                if (old is not null) src.Remove(old);
                src.AddDbContext<PgContext>(opt => { opt.UseNpgsql("Username=admin;Password=admin123;Persist Security Info=True;Database=cuteAudioDBTest;Host=192.168.1.110;Port=5433"); });

            });
        }

    
    }
}
