namespace VS.AuditHistory.Models
{
    public class FieldChangeModel
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
