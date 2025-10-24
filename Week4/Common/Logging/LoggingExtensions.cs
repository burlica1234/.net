using Microsoft.Extensions.Logging;

namespace Lab4.Common.Logging;

public static class LoggingExtensions
{
    public static void LogProductCreationMetrics(this ILogger logger, ProductCreationMetrics m)
    {
        logger.LogInformation(
            eventId: new EventId(LogEvents.ProductCreationCompleted, nameof(LogEvents.ProductCreationCompleted)),
            message: "Product metrics | OpId={OperationId} Name={Name} SKU={SKU} Category={Category} Valid(ms)={ValidMs} Db(ms)={DbMs} Total(ms)={TotalMs} Success={Success} Error={Error}",
            args: new object?[]
            {
                m.OperationId, m.ProductName, m.SKU, m.Category,
                (int)m.ValidationDuration.TotalMilliseconds,
                (int)m.DatabaseSaveDuration.TotalMilliseconds,
                (int)m.TotalDuration.TotalMilliseconds,
                m.Success, m.ErrorReason
            });
    }
}