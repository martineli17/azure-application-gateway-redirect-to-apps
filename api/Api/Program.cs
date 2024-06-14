using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 80, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;

                        });
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddCors(opt => opt.AddPolicy("FRONTEND", policy => policy.AllowAnyHeader().AllowAnyHeader().AllowAnyOrigin()));
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("FRONTEND");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
