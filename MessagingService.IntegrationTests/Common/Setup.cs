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
        public static (String usename, String password) SqlCredentials = ("sa", "thisisalongpassword123!");
        public static (String url, String username, String password) DockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");
        
        public static async Task GlobalSetup(DockerHelper dockerHelper)
        {
            ShouldlyConfiguration.DefaultTaskTimeout = TimeSpan.FromMinutes(1);
        }
    }
}
