﻿using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagingService.BusinessLogic.Tests.Mediator
{
    using BusinessLogic.Services;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using Testing;

    public class MediatorTests
    {
        private List<IBaseRequest> Requests = new List<IBaseRequest>();

        public MediatorTests() {
            this.Requests.Add(TestData.ResendEmailCommand);
            this.Requests.Add(TestData.SendEmailCommand);
            this.Requests.Add(TestData.SendSMSCommand);
            this.Requests.Add(TestData.ResendSMSCommand);
            this.Requests.Add(TestData.SendEmailCommandWithAttachments);
            this.Requests.Add(TestData.UpdateEmailMessageStatusCommand);
            this.Requests.Add(TestData.UpdateSMSMessageStatusCommand);
        }

        [Fact]
        public async Task Mediator_Send_RequestHandled() {
            Mock<IWebHostEnvironment> hostingEnvironment = new Mock<IWebHostEnvironment>();
            hostingEnvironment.Setup(he => he.EnvironmentName).Returns("Development");
            hostingEnvironment.Setup(he => he.ContentRootPath).Returns("/home");
            hostingEnvironment.Setup(he => he.ApplicationName).Returns("Test Application");

            ServiceRegistry services = new ServiceRegistry();
            Startup s = new Startup(hostingEnvironment.Object);
            Startup.Configuration = this.SetupMemoryConfiguration();

            this.AddTestRegistrations(services, hostingEnvironment.Object);
            s.ConfigureContainer(services);
            Startup.Container.AssertConfigurationIsValid(AssertMode.Full);

            List<String> errors = new List<String>();
            IMediator mediator = Startup.Container.GetService<IMediator>();
            foreach (IBaseRequest baseRequest in this.Requests) {
                try {
                    await mediator.Send(baseRequest);
                }
                catch(Exception ex) {
                    errors.Add(ex.Message);
                }
            }

            if (errors.Any() == true) {
                String errorMessage = String.Join(Environment.NewLine, errors);
                throw new Exception(errorMessage);
            }
        }

        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = TestData.GetStandardMemoryConfiguration();

            IConfigurationBuilder builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        private void AddTestRegistrations(ServiceRegistry services,
                                          IWebHostEnvironment hostingEnvironment) {
            services.AddLogging();
            DiagnosticListener diagnosticSource = new DiagnosticListener(hostingEnvironment.ApplicationName);
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddSingleton<DiagnosticListener>(diagnosticSource);
            services.AddSingleton<IWebHostEnvironment>(hostingEnvironment);
            services.AddSingleton<IHostEnvironment>(hostingEnvironment);
            services.AddSingleton<IConfiguration>(Startup.Configuration);

            services.OverrideServices(s => { s.AddSingleton<IMessagingDomainService, DummyMessagingDomainService>(); });
        }
    }
}
