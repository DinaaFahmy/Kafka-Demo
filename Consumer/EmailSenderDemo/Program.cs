using EmailSenderDemo.MQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmailSenderDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(service =>
                {
                    service.AddHostedServices();
                });
    }

    public static class DependencyInjection
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<EmailService>();  //Send queued emails
            services.AddHostedService<MQ_Service>();     //Consume and add to queues

            return services;
        }
    }
}
