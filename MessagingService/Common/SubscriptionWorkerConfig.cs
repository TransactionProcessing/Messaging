namespace MessagingService.Common;

using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class SubscriptionWorkerConfig
{
    public String WorkerName { get; set; }
    public String IncludeGroups { get; set; }
    public String IgnoreGroups { get; set; }
    public String IncludeStreams { get; set; }
    public String IgnoreStreams { get; set; }
    public Boolean Enabled { get; set; }
    public Int32 InflightMessages { get; set; }
    public Int32 InstanceCount { get; set; }
    public Boolean IsOrdered { get; set; }
}