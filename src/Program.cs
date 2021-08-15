using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
            var backuper = host.Services.GetService<Backuper>();
            await backuper.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                services.AddOptions();
                services.AddHttpClient();
                services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));
                services.AddSingleton<IFilesState, FilesState>();
                services.AddTransient<IFileReader, FileReader>();
                services.AddTransient<FilesBuilder>();
                services.AddTransient<FileUploader>();
                services.AddTransient<IStorageReader, StorageService>();
                services.AddTransient<IStorageCommand, StorageService>();
                services.AddTransient<Backuper>();
            });
    }
}
