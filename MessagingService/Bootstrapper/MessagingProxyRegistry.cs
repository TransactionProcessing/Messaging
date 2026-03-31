namespace MessagingService.Bootstrapper;

using BusinessLogic.Services.EmailServices;
using BusinessLogic.Services.EmailServices.Smtp2Go;
using BusinessLogic.Services.SMSServices;
using BusinessLogic.Services.SMSServices.TheSMSWorks;
using ClientProxyBase;
using Google.Api;
using Lamar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services.Email.IntegrationTest;
using Service.Services.SMSServices.IntegrationTest;
using Shared.General;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

[ExcludeFromCodeCoverage]
public class MessagingProxyRegistry : ServiceRegistry
{
    public MessagingProxyRegistry()
    {
        this.AddHttpContextAccessor();
        
        this.RegisterEmailProxy();
        this.RegisterSMSProxy();
    }

    private void RegisterEmailProxy() {
        // read the config setting 
        String emailProxy = ConfigurationReader.GetValue("AppSettings", "EmailProxy");

        if (emailProxy == "Smtp2Go") {
            Smtp2GConfig config = Startup.Configuration.GetSection("AppSettings:Smtp2Go").Get<Smtp2GConfig>() ?? throw new InvalidOperationException("Smtp2Go config missing");
            this.AddSingleton(config);
            this.RegisterHttpClient<IEmailServiceProxy, Smtp2GoProxy>();
        }
        else {
            this.AddSingleton<IEmailServiceProxy, IntegrationTestEmailServiceProxy>();
        }
    }

    private void RegisterSMSProxy() {
        // read the config setting 
        String smsProxy = ConfigurationReader.GetValue("AppSettings", "SMSProxy");

        if (smsProxy == "TheSMSWorks") {
            SmsWorksConfig config = Startup.Configuration.GetSection("AppSettings:TheSmsWorks").Get<SmsWorksConfig>() ?? throw new InvalidOperationException("SmsWorks config missing");
            this.AddSingleton(config);
            this.RegisterHttpClient<ISMSServiceProxy, TheSmsWorksProxy>();
        }
        else {
            this.AddSingleton<ISMSServiceProxy, IntegrationTestSMSServiceProxy>();
        }
    }
}