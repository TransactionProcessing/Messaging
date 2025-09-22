using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.Tests
{
    using System.Threading;
    using Controllers;
    using MessagingService.EmailMessage.DomainEvents;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Newtonsoft.Json;
    using Shared.EventStore.EventHandling;
    using Shared.General;
    using Shared.Logger;
    using Shouldly;
    using Xunit;

    public class ControllerTests
    {
        public ControllerTests() {
            Logger.Initialise(new NullLogger());
        }
        [Fact]
        public async Task DomainEventController_EventIdNotPresentInJson_ErrorThrown() {
            Mock<IDomainEventHandlerResolver> resolver = new();
            TypeMap.AddType<ResponseReceivedFromEmailProviderEvent>("ResponseReceivedFromEmailProviderEvent");
            DefaultHttpContext httpContext = new();
            httpContext.Request.Headers["eventType"] = "ResponseReceivedFromEmailProviderEvent";
            DomainEventController controller = new(resolver.Object)
                                               {
                                                   ControllerContext = new ControllerContext()
                                                                       {
                                                                           HttpContext = httpContext
                                                                       }
                                               };
            String json = "{\r\n  \"messageId\": \"811bb215-0d99-4639-ac3a-195ba4a47449\",\r\n  \"providerRequestReference\": \"d8f669fa-675d-11ec-8b59-f23c92160e3c\"\r\n}\t";
            Object request = JsonConvert.DeserializeObject(json);
            ArgumentException ex = Should.Throw<ArgumentException>(() => controller.PostEventAsync(request, CancellationToken.None));
            ex.Message.ShouldBe("Domain Event must contain an Event Id");
        }

        [Fact]
        public async Task DomainEventController_EventIdPresentInJson_NoErrorThrown()
        {
            Mock<IDomainEventHandlerResolver> resolver = new();
            TypeMap.AddType<ResponseReceivedFromEmailProviderEvent>("ResponseReceivedFromEmailProviderEvent");
            DefaultHttpContext httpContext = new();
            httpContext.Request.Headers["eventType"] = "ResponseReceivedFromEmailProviderEvent";
            DomainEventController controller = new(resolver.Object)
                                               {
                                                   ControllerContext = new ControllerContext()
                                                                       {
                                                                           HttpContext = httpContext
                                                                       }
                                               };
            String json = "{\r\n  \"messageId\": \"811bb215-0d99-4639-ac3a-195ba4a47449\",\r\n  \"providerRequestReference\": \"d8f669fa-675d-11ec-8b59-f23c92160e3c\",\r\n  \"eventId\": \"123bb215-0d99-4639-ac3a-195ba4a47449\"\r\n}\t";
            Object request = JsonConvert.DeserializeObject(json);
            Should.NotThrow(async () => await controller.PostEventAsync(request, CancellationToken.None));
        }
    }
}
