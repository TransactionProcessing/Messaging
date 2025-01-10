using MessagingService.DataTransferObjects;
using SimpleResults;

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
        this.AddSingleton<IRequestHandler<EmailCommands.SendEmailCommand, Result<Guid>>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<SMSCommands.SendSMSCommand, Result<Guid>>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<EmailCommands.ResendEmailCommand,Result>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<SMSCommands.ResendSMSCommand,Result>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<SMSCommands.UpdateMessageStatusCommand, Result>, MessagingRequestHandler>();
        this.AddSingleton<IRequestHandler<EmailCommands.UpdateMessageStatusCommand, Result>, MessagingRequestHandler>();

        this.AddSingleton<Func<String, String>>(container => (serviceName) =>
                                                             {
                                                                 return ConfigurationReader.GetBaseServerUri(serviceName).OriginalString;
                                                             });
    }
}