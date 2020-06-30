using Microsoft.Xrm.Sdk;

namespace VS.AuditHistory.Interfaces
{
    public interface ID365Access
    {
        IOrganizationService CrmService { get; set; }

        IOrganizationService GetCrmService();
    }
}