using System.Net;
using System.Threading;
using Microsoft.Xrm.Sdk;
using VS.AuditHistory.Interfaces;

namespace VS.AuditHistory
{
    public class D365Access : ID365Access
    {
        private readonly string _connectionString;

        private static readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();

        public IOrganizationService CrmService { get; set; }

        public D365Access(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IOrganizationService GetCrmService()
        {
            CacheLock.EnterReadLock();

            try
            {
                if (CrmService == null)
                {
                    CrmService = GetService();
                }
            }
            finally
            {
                CacheLock.ExitReadLock();
            }

            return CrmService;
        }

        private IOrganizationService GetService()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(_connectionString);
            var service = conn.OrganizationWebProxyClient ?? conn.OrganizationServiceProxy as IOrganizationService;

            return service;
        }

    }
}