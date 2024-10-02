using WebTestJsonConverter;
using WebTestJsonConverter.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Linq.JsonPath;
using Newtonsoft.Json;

namespace WebTestJsonConverter;

public class Program
{
    public static void Main(string[] args)
    {
        ThreadPool.SetMinThreads(400, 400);
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}