using Abno.Data;
using Abno.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Abno
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var environment = host.Services.GetRequiredService<IHostEnvironment>();
            var env = environment.EnvironmentName;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var seeder = services.GetRequiredService<Seeder>();

                    await seeder.IdentitySeed(services, context);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            // Run the application
            await host.RunAsync();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
