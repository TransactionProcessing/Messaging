namespace MessagingService.Bootstrapper;

using System;
using System.Diagnostics.CodeAnalysis;
using BusinessLogic.RequestHandlers;
using BusinessLogic.Requests;
using Lamar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.General;

[ExcludeFromCodeCoverage]
public class MediatorRegistry : ServiceRegistry
{
    public MediatorRegistry()
    {
        this.AddTransient<IMediator, Mediator>();
        this.AddSingleton<IRequestHandler<SendEmailRequest>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<SendSMSRequest>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<ResendEmailRequest>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<ResendSMSRequest>, MessagingRequestHandler>();

        this.AddSingleton<Func<String, String>>(container => (serviceName) =>
                                                             {
                                                                 return ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
                                                             });

        // request & notification handlers
        this.AddTransient<ServiceFactory>(context =>
                                          {
                                              return t => context.GetService(t);
                                          });
    }
}