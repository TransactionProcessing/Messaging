﻿namespace MessagingService.Bootstrapper;

using System;
using BusinessLogic.RequestHandlers;
using BusinessLogic.Requests;
using Lamar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.General;

public class MediatorRegistry : ServiceRegistry
{
    public MediatorRegistry()
    {
        this.AddTransient<IMediator, Mediator>();
        this.AddSingleton<IRequestHandler<SendEmailRequest, String>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<SendSMSRequest, String>, MessagingRequestHandler>();

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