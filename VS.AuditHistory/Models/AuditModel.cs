using System;
using System.Collections.Generic;

namespace VS.AuditHistory.Models
{
    public class AuditModel
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public ReferenceModel ChangedById { get; set; }
        public int EventCode { get; set; }
        public string Event { get; set; }
        public List<FieldChangeModel> FieldChanges { get; set; }
    }

    public enum AuditActions
    {
        Create = 1,
        Update,
        Delete,
        Access
    }
}
