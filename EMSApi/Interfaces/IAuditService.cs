namespace EMSApi.Services
{
    public interface IAuditService
    {
        Task LogAsync(string entityName, int entityId, string action, string performedBy, string changes);
    }
}
