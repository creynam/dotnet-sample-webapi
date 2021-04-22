using System;
// using Microsoft.ApplicationInsights.Channel;
// using Microsoft.ApplicationInsights.DataContracts;
// using Microsoft.ApplicationInsights.Extensibility;

namespace WebApi.Filters
{
    // public class NotFoundTelemetryProcessor : ITelemetryProcessor
    // {
    //     private readonly ITelemetryProcessor _next;

    //     public NotFoundTelemetryProcessor(ITelemetryProcessor next)
    //     {
    //         _next = next ?? throw new ArgumentNullException(nameof(next));
    //     }

    //     public void Process(ITelemetry item)
    //     {
    //         // Ignore item if we got a 404 response, this is often expected
    //         if (item is RequestTelemetry reqTel)
    //             if (reqTel.ResponseCode.Equals("404", StringComparison.OrdinalIgnoreCase))
    //                 return;

    //         _next.Process(item);
    //     }
    // }
}