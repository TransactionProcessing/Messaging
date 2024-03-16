using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using NLog;
using Shared.Logger;
using Shouldly;
using System;

namespace MessagingService.IntegrationTests.Common
{
    using System.Threading.Tasks;
    using global::Shared.IntegrationTesting;
    using Reqnroll;

    [Binding]
    public class Setup
    {
        public static IContainerService DatabaseServerContainer;
        public static INetworkService DatabaseServerNetwork;
        public static (String usename, String password) SqlCredentials = ("sa", "thisisalongpassword123!");
        public static (String url, String username, String password) DockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");
        
        public static async Task GlobalSetup(DockerHelper dockerHelper)
        {
            ShouldlyConfiguration.DefaultTaskTimeout = TimeSpan.FromMinutes(1);
            dockerHelper.SqlCredentials = Setup.SqlCredentials;
            dockerHelper.DockerCredentials = Setup.DockerCredentials;
            dockerHelper.SqlServerContainerName = "sharedsqlserver";

            await Retry.For(async () => {
                                Setup.DatabaseServerNetwork = dockerHelper.SetupTestNetwork("sharednetwork", true);
                                Setup.DatabaseServerContainer = await dockerHelper.SetupSqlServerContainer(Setup.DatabaseServerNetwork);
                            },TimeSpan.FromSeconds(10));
        }

        public static String GetConnectionString(String databaseName)
        {
            return $"server={Setup.DatabaseServerContainer.Name};database={databaseName};user id={Setup.SqlCredentials.usename};password={Setup.SqlCredentials.password}";
        }

        public static String GetLocalConnectionString(String databaseName)
        {
            Int32 databaseHostPort = Setup.DatabaseServerContainer.ToHostExposedEndpoint("1433/tcp").Port;

            return $"server=localhost,{databaseHostPort};database={databaseName};user id={Setup.SqlCredentials.usename};password={Setup.SqlCredentials.password}";
        }
    }
}
