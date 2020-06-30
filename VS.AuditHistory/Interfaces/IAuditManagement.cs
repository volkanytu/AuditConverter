using Microsoft.Crm.Sdk.Messages;
using VS.AuditHistory.Models;

namespace VS.AuditHistory.Interfaces
{
    internal interface IAuditManagement
    {
        AuditModel ConvertAuditResponseToAuditModel(RetrieveAuditDetailsResponse response);
    }
}