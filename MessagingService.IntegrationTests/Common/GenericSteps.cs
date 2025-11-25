using NLog;
using Shared.IntegrationTesting;
using Shared.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.IntegrationTests.Common
{
    using Reqnroll;

    [Binding]
    [Scope(Tag = "base")]
    public class GenericSteps
    {
        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        public GenericSteps(ScenarioContext scenarioContext,
                            TestingContext testingContext)
        {
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;
        }

        [BeforeScenario]
        public async Task StartSystem()
        {
            
            // Initialise a logger
            String scenarioName = this.ScenarioContext.ScenarioInfo.Title.Replace(" ", "");
            NlogLogger logger = new();
            logger.Initialise(LogManager.GetLogger(scenarioName), scenarioName);
            LogManager.AddHiddenAssembly(typeof(NlogLogger).Assembly);

            DockerServices dockerServices = DockerServices.SqlServer | DockerServices.MessagingService | DockerServices.EventStore | DockerServices.SecurityService;

            this.TestingContext.DockerHelper = new();
            this.TestingContext.DockerHelper.Logger = logger;
            this.TestingContext.Logger = logger;
            this.TestingContext.DockerHelper.RequiredDockerServices = dockerServices;
            this.TestingContext.Logger.LogInformation("About to Start Global Setup");

            await Setup.GlobalSetup(this.TestingContext.DockerHelper);

            this.TestingContext.DockerHelper.DockerCredentials = Setup.DockerCredentials;
            this.TestingContext.DockerHelper.SqlCredentials = Setup.SqlCredentials;
            this.TestingContext.DockerHelper.SqlServerContainerName = "sharedsqlserver";

            this.TestingContext.DockerHelper.SetImageDetails(ContainerType.MessagingService, ("messagingservice", false));

            this.TestingContext.Logger = logger;
            this.TestingContext.Logger.LogInformation("About to Start Containers for Scenario Run");
            await this.TestingContext.DockerHelper.StartContainersForScenarioRun(scenarioName, dockerServices).ConfigureAwait(false);
            this.TestingContext.Logger.LogInformation("Containers for Scenario Run Started");

        }

        [AfterScenario]
        public async Task StopSystem(){
            DockerServices dockerSharedServices = DockerServices.None;
            this.TestingContext.Logger.LogInformation("About to Stop Containers for Scenario Run");
            await this.TestingContext.DockerHelper.StopContainersForScenarioRun(dockerSharedServices).ConfigureAwait(false);
            this.TestingContext.Logger.LogInformation("Containers for Scenario Run Stopped");
        }
    }
}
