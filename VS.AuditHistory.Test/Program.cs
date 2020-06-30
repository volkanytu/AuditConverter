using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS.AuditHistory.Interfaces;

namespace VS.AuditHistory.Test
{
    class Program
    {
        private static ID365Access d365Access;

        static void Main(string[] args)
        {
            d365Access = new D365Access("CONNECTION_STRING_HERE");

            var auditConverter = new AuditConverter(d365Access);

            //Retrieve audit histories you want to convert
            var auditHistories = GetAuditLogs();

            foreach (var auditHistory in auditHistories.Entities)
            {
                var auditModel = auditConverter.ConvertToAuditModel(auditHistory.Id);

                string modelJson = JsonConvert.SerializeObject(auditModel, Formatting.Indented);

                Console.WriteLine(modelJson);

            }
        }

        private static EntityCollection GetAuditLogs()
        {
            var service = d365Access.GetCrmService();

            var qExp = new QueryExpression("audit")
            {
                ColumnSet = new ColumnSet(true),
                Orders =
                {
                    new OrderExpression("createdon",OrderType.Descending)
                }
            };

            return service.RetrieveMultiple(qExp);
        }
    }
}
