using System;
using VS.AuditHistory.Models;

namespace VS.AuditHistory
{
    public interface IAuditConverter
    {
        AuditModel ConvertToAuditModel(Guid auditId);
    }
}