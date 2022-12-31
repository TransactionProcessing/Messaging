namespace MessagingService.Bootstrapper;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using BusinessLogic.Services.EmailServices;
using BusinessLogic.Services.EmailServices.Smtp2Go;
using BusinessLogic.Services.SMSServices;
using BusinessLogic.Services.SMSServices.TheSMSWorks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Service.Services.Email.IntegrationTest;
using Service.Services.SMSServices.IntegrationTest;
using Shared.General;

[ExcludeFromCodeCoverage]
public class MessagingProxyRegistry : ServiceRegistry
{
    public MessagingProxyRegistry()
    {
        SocketsHttpHandler httpMessageHandler = new SocketsHttpHandler
                                                {
                                                    SslOptions =
                                                    {
                                                        RemoteCertificateValidationCallback = (sender,
                                                                                               certificate,
                                                                                               chain,
                                                                                               errors) => true,
                                                    }
                                                };
        HttpClient httpClient = new HttpClient(httpMessageHandler);
        this.AddSingleton(httpClient);

        this.RegisterEmailProxy();
        this.RegisterSMSProxy();
    }
    /// <summary>
    /// Registers the email proxy.
    /// </summary>
    /// <param name="services">The services.</param>
    private void RegisterEmailProxy()
    {
        // read the config setting 
        String emailProxy = ConfigurationReader.GetValue("AppSettings", "EmailProxy");

        if (emailProxy == "Smtp2Go")
        {
            this.AddSingleton<IEmailServiceProxy, Smtp2GoProxy>();
        }
        else
        {
            this.AddSingleton<IEmailServiceProxy, IntegrationTestEmailServiceProxy>();
        }
    }

    private void RegisterSMSProxy()
    {
        // read the config setting 
        String emailProxy = ConfigurationReader.GetValue("AppSettings", "SMSProxy");

        if (emailProxy == "TheSMSWorks")
        {
            this.AddSingleton<ISMSServiceProxy, TheSmsWorksProxy>();
        }
        else
        {
            this.AddSingleton<ISMSServiceProxy, IntegrationTestSMSServiceProxy>();
        }
    }
}