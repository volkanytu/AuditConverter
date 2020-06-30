using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using VS.AuditHistory.Interfaces;
using VS.AuditHistory.Models;

namespace VS.AuditHistory
{
    public class AuditManagement : IAuditManagement
    {
        private readonly IOrganizationService _service;
        private readonly ID365Access _d365Access;

        public AuditManagement(ID365Access d365Access)
        {
            _d365Access = d365Access;
            _service = d365Access.GetCrmService();
        }

        public AuditModel ConvertAuditResponseToAuditModel(RetrieveAuditDetailsResponse response)
        {
            var newValue = ((AttributeAuditDetail)response.AuditDetail).NewValue;
            var oldValue = ((AttributeAuditDetail)response.AuditDetail).OldValue;
            var auditRecord = response.AuditDetail.AuditRecord;
            var entityObject = (EntityReference)auditRecord[Constants.Attr_ObjectId];

            var model = new AuditModel
            {
                Id = auditRecord.Id,
                Date = (DateTime)auditRecord[Constants.Attr_CreatedOn],
                ChangedById = ToReferenceModel((EntityReference)auditRecord[Constants.Àttr_UserId]),
                EventCode = ((OptionSetValue)auditRecord[Constants.Attr_Action]).Value,
                Event = ((AuditActions)((OptionSetValue)auditRecord[Constants.Attr_Action]).Value).ToString(),
                FieldChanges = new List<FieldChangeModel>()
            };

            foreach (var attribute in newValue.Attributes)
            {
                var valueChange = new FieldChangeModel
                {
                    FieldName = attribute.Key,
                    NewValue = ConvertToModel(attribute.Value, entityObject.LogicalName, attribute.Key)
                };

                valueChange.OldValue = oldValue.Attributes.Contains(attribute.Key) ? ConvertToModel(oldValue[attribute.Key], entityObject.LogicalName, attribute.Key) : null;

                if (attribute.Value != null)
                {
                    valueChange.FieldType = attribute.Value.GetType().Name;
                }
                else
                {
                    valueChange.FieldType = oldValue[attribute.Key].GetType().Name;
                }

                model.FieldChanges.Add(valueChange);
            }

            return model;
        }

        #region | PRIVATE METHODS |
        private string GetOptionsSetText(string entityName, string attributeName, int value)
        {

            var retrieveAttributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            var retrieveAttributeResponse = (RetrieveAttributeResponse)_service.Execute(retrieveAttributeRequest);
            var retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
            var optionList = retrievedPicklistAttributeMetadata.OptionSet.Options;

            return optionList.Where(o => o.Value == value).FirstOrDefault().Label.LocalizedLabels.FirstOrDefault().Label;
        }
        private ReferenceModel ToReferenceModel(EntityReference value)
        {
            if (value == null)
            {
                return null;
            }

            return new ReferenceModel()
            {
                Id = value.Id,
                LogicalName = value.LogicalName,
                Name = value.Name
            };
        }

        private OptionModel ToOptionModel(OptionSetValue value, string entityName, string attributeName)
        {
            if (value == null)
            {
                return null;
            }

            var text = GetOptionsSetText(entityName, attributeName, value.Value);

            return new OptionModel()
            {
                Value = value.Value,
                Text = text
            };
        }

        private decimal? ToDecimalValue(Money value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value;
        }

        private object ConvertToModel(object value, string entityName, string attributeName)
        {
            if (value == null)
            {
                return null;
            }

            if (value is EntityReference)
            {
                return ToReferenceModel((EntityReference)value);
            }
            else if (value is OptionSetValue)
            {
                return ToOptionModel((OptionSetValue)value, entityName, attributeName);
            }
            else if (value is Money)
            {
                return ToDecimalValue((Money)value);
            }

            return value;
        }
        #endregion


    }
}
