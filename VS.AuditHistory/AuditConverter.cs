using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS.AuditHistory.Interfaces;
using VS.AuditHistory.Models;

namespace VS.AuditHistory
{
    public class AuditConverter : IAuditConverter
    {
        private readonly ID365Access _d365Access;
        private readonly IAuditManagement _auditManagement;

        public AuditConverter(ID365Access d365Access)
        {
            _d365Access = d365Access;

            _auditManagement = new AuditManagement(d365Access);
        }

        public AuditModel ConvertToAuditModel(Guid auditId)
        {
            var service = _d365Access.GetCrmService();

            var request = new RetrieveAuditDetailsRequest()
            {
                AuditId = auditId
            };

            var result = service.Execute(request);

            var model = _auditManagement.ConvertAuditResponseToAuditModel((RetrieveAuditDetailsResponse)result);

            return model;
        }
    }
}
