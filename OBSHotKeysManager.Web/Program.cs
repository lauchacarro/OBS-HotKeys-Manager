using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace OBSHotKeysManager.Web
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
                    webBuilder.UseStartup<Startup>().UseUrls(GetUrl(args));
                });

        public static string GetUrl(string[] args) => $"http://0.0.0.0:{(args.Length > 0 ? int.Parse(args[0]) : 5000)}";
    }
}
