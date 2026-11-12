using Imposter.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using MessagingService.BusinessLogic.Services.EmailServices;
using MessagingService.BusinessLogic.Services.SMSServices;
using TransactionProcessing.Shared.DomainDrivenDesign.EventSourcing;
using TransactionProcessing.Shared.EventStore.Aggregate;

[assembly: GenerateImposter(typeof(IAggregateRepository<,>))]
[assembly: GenerateImposter(typeof(IEmailServiceProxy))]
[assembly: GenerateImposter(typeof(ISMSServiceProxy))]
[assembly: GenerateImposter(typeof(IMediator))]
[assembly: GenerateImposter(typeof(IWebHostEnvironment))]
