
using EMSApi.Models;

namespace EMSApi.Services
{
    public class AuditService : IAuditService
    {
        private readonly EMSdbContext _context;
        private readonly ILogger<AuditService> _logger;
        public AuditService(EMSdbContext context,ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task LogAsync(string entityName, int entityId, string action, string performedBy, string changes)
        {
            var log = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                PerformedBy = performedBy,
                Changes = changes,
                PerformedAt = DateTime.UtcNow
            };

            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Audit: {action} -> {entityName} ({entityId}) by {performedBy}");
        }
    }
}
