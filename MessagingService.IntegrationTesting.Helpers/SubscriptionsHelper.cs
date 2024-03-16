using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingService.IntegrationTesting.Helpers
{
    public static class SubscriptionsHelper
    {
        public static List<(String streamName, String groupName, Int32 maxRetries)> GetSubscriptions(){
            List<(String streamName, String groupName, Int32 maxRetries)> subscriptions = new(){
                                                                                                   ("$ce-EmailAggregate", "Messaging Service", 0),
                                                                                                   ("$ce-SMSAggregate", "Messaging Service", 0)
                                                                                               };
            return subscriptions;
        }
    }
}
