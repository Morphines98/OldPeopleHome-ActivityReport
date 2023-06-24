using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Azure.Storage.Queues;
using Azure.Storage;
using SendChatPushNotifications.Services;

using Azure.Identity;

[assembly: FunctionsStartup(typeof(SendChatPushNotifications.Startup))]
namespace SendChatPushNotifications
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      var configuration = builder.GetContext().Configuration;

      var dbConnectionString = configuration.GetConnectionString("DbConnectionString");

      builder.Services.AddSingleton<MySqlConnectionDbConnection>((s) =>
      {
        return new MySqlConnectionDbConnection(dbConnectionString);
      });
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
      var builtConfig = builder.ConfigurationBuilder.Build();
      var keyVaultEndpoint = builtConfig["VaultUri"];

      builder.ConfigurationBuilder
       .SetBasePath(Environment.CurrentDirectory)
       .AddJsonFile("local.settings.json", true)
       .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
       .AddEnvironmentVariables();

      if (!string.IsNullOrEmpty(keyVaultEndpoint))
      {
        builder.ConfigurationBuilder.AddAzureKeyVault(new Uri(keyVaultEndpoint)
                            ,new DefaultAzureCredential());
      }
      builder.ConfigurationBuilder.Build();
    }
  }
}

