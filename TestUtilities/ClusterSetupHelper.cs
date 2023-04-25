using System.Reflection;
using Example.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestUtilities
{
    public class ClusterSetupHelper
    {
        private readonly DbSetupHelper _dbSetupHelper;
        private readonly string _dbNameBase;

        public ClusterSetupHelper(string dbServer, string user, string password)
        {
            _dbNameBase = "exampleutest";
            _dbSetupHelper = new DbSetupHelper(dbServer, user, password);
        }

        public void Setup(string dbSetup = "dbsetup.sql")
        {
            string tables = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", dbSetup));
            _dbSetupHelper.SetupDb(_dbNameBase, tables);
        }

        public void TearDown()
        {
            _dbSetupHelper.TearDownDb(_dbNameBase);
        }

        public IServiceProvider CreateServiceProvider(Action<ServiceCollection> additionalConfigurer)
        {
            ServiceCollection serviceCollection = new();
            serviceCollection.AddLogging();
            additionalConfigurer?.Invoke(serviceCollection);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("TestCaseDefaultConnection") ?? string.Empty;
            serviceCollection.AddDbContext<ExamplesContext>(
               option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            serviceCollection.AddScoped<IConfiguration>(_ => configuration);
            return serviceCollection.BuildServiceProvider();
        }
    }
}