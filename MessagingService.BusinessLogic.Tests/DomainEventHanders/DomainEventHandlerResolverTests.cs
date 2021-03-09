namespace MessagingService.BusinessLogic.Tests.DomainEventHanders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EmailMessage.DomainEvents;
    using EventHandling;
    using Moq;
    using Shared.EventStore.EventHandling;
    using Shouldly;
    using Testing;
    using Xunit;

    public class DomainEventHandlerResolverTests
    {
        [Fact]
        public void DomainEventHandlerResolver_CanBeCreated_IsCreated()
        {
            Dictionary<String, String[]> eventHandlerConfiguration = new Dictionary<String, String[]>();

            eventHandlerConfiguration.Add("TestEventType1", new[] { "MessagingService.BusinessLogic.EventHandling.EmailDomainEventHandler, MessagingService.BusinessLogic" });

            Mock<IDomainEventHandler> domainEventHandler = new Mock<IDomainEventHandler>();
            Func<Type, IDomainEventHandler> createDomainEventHandlerFunc = (type) => { return domainEventHandler.Object; };
            DomainEventHandlerResolver resolver = new DomainEventHandlerResolver(eventHandlerConfiguration, createDomainEventHandlerFunc);

            resolver.ShouldNotBeNull();
        }

        [Fact]
        public void DomainEventHandlerResolver_CanBeCreated_InvalidEventHandlerType_ErrorThrown()
        {
            Dictionary<String, String[]> eventHandlerConfiguration = new Dictionary<String, String[]>();

            eventHandlerConfiguration.Add("TestEventType1", new String[] { "MessagingService.BusinessLogic.EventHandling.NonExistantDomainEventHandler" });

            Mock<IDomainEventHandler> domainEventHandler = new Mock<IDomainEventHandler>();
            Func<Type, IDomainEventHandler> createDomainEventHandlerFunc = (type) => { return domainEventHandler.Object; };

            Should.Throw<NotSupportedException>(() => new DomainEventHandlerResolver(eventHandlerConfiguration, createDomainEventHandlerFunc));
        }

        [Fact]
        public void DomainEventHandlerResolver_GetDomainEventHandlers_ResponseReceivedFromProviderEvent_EventHandlersReturned()
        {
            String handlerTypeName = "MessagingService.BusinessLogic.EventHandling.EmailDomainEventHandler, MessagingService.BusinessLogic";
            Dictionary<String, String[]> eventHandlerConfiguration = new Dictionary<String, String[]>();

            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent = TestData.ResponseReceivedFromEmailProviderEvent;

            eventHandlerConfiguration.Add(responseReceivedFromProviderEvent.GetType().Name, new [] { handlerTypeName });

            Mock<IDomainEventHandler> domainEventHandler = new Mock<IDomainEventHandler>();
            Func<Type, IDomainEventHandler> createDomainEventHandlerFunc = (type) => { return domainEventHandler.Object; };

            DomainEventHandlerResolver resolver = new DomainEventHandlerResolver(eventHandlerConfiguration, createDomainEventHandlerFunc);

            List<IDomainEventHandler> handlers = resolver.GetDomainEventHandlers(responseReceivedFromProviderEvent);

            handlers.ShouldNotBeNull();
            handlers.Any().ShouldBeTrue();
            handlers.Count.ShouldBe(1);
        }

        [Fact]
        public void DomainEventHandlerResolver_GetDomainEventHandlers_ResponseReceivedFromProviderEvent_EventNotConfigured_EventHandlersReturned()
        {
            String handlerTypeName = "MessagingService.BusinessLogic.EventHandling.EmailDomainEventHandler, MessagingService.BusinessLogic";
            Dictionary<String, String[]> eventHandlerConfiguration = new Dictionary<String, String[]>();

            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent = TestData.ResponseReceivedFromEmailProviderEvent;

            eventHandlerConfiguration.Add("RandomEvent", new String[] { handlerTypeName });
            Mock<IDomainEventHandler> domainEventHandler = new Mock<IDomainEventHandler>();
            Func<Type, IDomainEventHandler> createDomainEventHandlerFunc = (type) => { return domainEventHandler.Object; };

            DomainEventHandlerResolver resolver = new DomainEventHandlerResolver(eventHandlerConfiguration, createDomainEventHandlerFunc);

            List<IDomainEventHandler> handlers = resolver.GetDomainEventHandlers(responseReceivedFromProviderEvent);

            handlers.ShouldBeNull();
        }

        [Fact]
        public void DomainEventHandlerResolver_GetDomainEventHandlers_ResponseReceivedFromProviderEvent_NoHandlersConfigured_EventHandlersReturned()
        {
            Dictionary<String, String[]> eventHandlerConfiguration = new Dictionary<String, String[]>();

            ResponseReceivedFromEmailProviderEvent responseReceivedFromProviderEvent = TestData.ResponseReceivedFromEmailProviderEvent;
            Mock<IDomainEventHandler> domainEventHandler = new Mock<IDomainEventHandler>();

            Func<Type, IDomainEventHandler> createDomainEventHandlerFunc = (type) => { return domainEventHandler.Object; };

            DomainEventHandlerResolver resolver = new DomainEventHandlerResolver(eventHandlerConfiguration, createDomainEventHandlerFunc);

            List<IDomainEventHandler> handlers = resolver.GetDomainEventHandlers(responseReceivedFromProviderEvent);

            handlers.ShouldBeNull();
        }

    }
}