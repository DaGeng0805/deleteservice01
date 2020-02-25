using System.Threading.Tasks;
using IStrong.EC.Abstractions.Extensions;
using IStrong.EC.DAO;
using IStrong.EC.TaskManager;
using IStrong.EC.DAO.Mapping.Xml;
using IStrong.EC.TaskManager.JobStorage.Json;
using Microsoft.Extensions.DependencyInjection;

namespace IStrong.EC.Scheduler.DecryptionService
{
    /// <summary>
    /// Program
    /// </summary>
    internal class Program
    {
        static async Task Main()
        {
            await IStrong.EC.TaskManager.Scheduler.Init((hostContext, services, logger) =>
            {
                //安装nuget包：Microsoft.Extensions.Caching.Memory 2.1.0
                services.AddMemoryCache();
                //安装nuget包：IStrong.EC.DAO.Mapping.Xml
                services.AddXmlMappingProvider();
                services.AddDatabaseFeature();
                //默认使用Config目录下的jobs.json存储job信息，安装nuget包：IStrong.EC.TaskManager.JobStorage.Json
                services.AddJsonJobStorage(new JsonJobStorageProviderOption() { Logger = logger });

            });
        }
    }
}
