using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public partial class EntityMapper
    {
        public void MapAttributes(Entity entity, object[] data, WriteToDynamicsCrmConfiguration configuration)
        {
            foreach (var dataMapping in this.mappings)
            {
                AttributeMetadata attributeMetadata = this.attributeMetadataDictionary[dataMapping.Target];
                object obj = data[this.columnMetadataDictionary[dataMapping.Source].ColumnIndex];

                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                {
                    entity.Attributes.Add(dataMapping.Target, null);
                    continue;
                }

                switch (attributeMetadata.AttributeType.Value)
                {
                    case AttributeTypeCode.BigInt:
                        long longValue = Convert.ToInt64(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, longValue);
                        break;

                    case AttributeTypeCode.Boolean:
                        bool booleanValue = Convert.ToBoolean(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, booleanValue);
                        break;

                    case AttributeTypeCode.Customer:

                        if (configuration.LookupResolve == LookupResolve.Guid)
                        {
                            LookupAttributeMetadata clookupMetadata = attributeMetadata as LookupAttributeMetadata;
                            Guid clookupId = new Guid(obj.ToString());
                            entity.Attributes.Add(dataMapping.Target, new EntityReference(clookupMetadata.Targets[0], clookupId));
                        }
                        if (configuration.LookupResolve == LookupResolve.All)
                        {
                            throw new Exception("Direct Customer Attribute Mapping not supported!");
                        }

                        break;
                    case AttributeTypeCode.DateTime:

                        if (dataMapping.Automap)
                        {
                            entity.Attributes.Add(attributeMetadata.LogicalName, obj);
                        }
                        else if(obj.GetType() == typeof(DateTime))
                        {
                            entity.Attributes.Add(dataMapping.Target, (DateTime)obj);
                        }
                        else
                        {
                            DateTime? dt = DateTimeHelper.ConvertStringToDateTime(obj.ToString(), dataMapping.ValueFormat);
                            if (dt != null)
                            {
                                entity.Attributes.Add(attributeMetadata.LogicalName, dt);
                            }
                            else
                            {
                                throw new Exception("Could not convert value " + obj.ToString() + " to datetime. Please correct value or check valueformat!");
                            }
                        }
                        break;

                    case AttributeTypeCode.Decimal:
                        try
                        {
                            decimal decimalValue = Convert.ToDecimal(obj.ToString());
                            entity.Attributes.Add(dataMapping.Target, decimalValue);
                        }
                        catch(FormatException)
                        {
                            throw new FormatException("Could not convert value '" + obj.ToString() + "' to decimal.");
                        }
                        break;

                    case AttributeTypeCode.Double:
                        double doubleValue = Convert.ToDouble(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, doubleValue);
                        break;

                    case AttributeTypeCode.Integer:
                        int intValue = Convert.ToInt32(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, intValue);
                        break;

                    case AttributeTypeCode.Owner:
                        if (obj is EntityReference)
                        {
                            entity.Attributes.Add(dataMapping.Target, obj);
                        }
                        else
                        {
                            LookupAttributeMetadata olookupMetadata = attributeMetadata as LookupAttributeMetadata;
                            Guid olookupId = new Guid(obj.ToString());
                            entity.Attributes.Add(dataMapping.Target, new EntityReference(olookupMetadata.Targets[0], olookupId));
                        }
                        break;

                    case AttributeTypeCode.Lookup:
                        if (configuration.LookupResolve == LookupResolve.Guid || configuration.LookupResolve == LookupResolve.All)
                        {
                            if (obj is EntityReference)
                            {
                                entity.Attributes.Add(dataMapping.Target, obj);
                            }
                            else
                            {
                                LookupAttributeMetadata lookupMetadata = attributeMetadata as LookupAttributeMetadata;
                                Guid lookupId = new Guid(obj.ToString());
                                entity.Attributes.Add(dataMapping.Target, new EntityReference(lookupMetadata.Targets[0], lookupId));
                            }
                        }

                        break;

                    case AttributeTypeCode.Money:

                        if (obj is Money)
                        {
                            entity.Attributes.Add(dataMapping.Target, obj);
                        }
                        else
                        {
                            decimal moneyValue = Convert.ToDecimal(obj.ToString());
                            entity.Attributes.Add(dataMapping.Target, new Money(moneyValue));
                        }
                        
                        break;

                    case AttributeTypeCode.Memo:
                    case AttributeTypeCode.String:
                        string stringValue = obj.ToString();
                        if (string.IsNullOrEmpty(stringValue))
                        {
                            stringValue = null;
                        }
                        entity.Attributes.Add(dataMapping.Target, stringValue);
                        break;

                    case AttributeTypeCode.Status:
                    case AttributeTypeCode.Picklist:
                        IntegrationTool.Module.WriteToDynamicsCrm.SDK.PicklistMapping picklistMapping = this.picklistMappings.Where(t => t.LogicalName == dataMapping.Target).First();
                        int optionValue = -1;
                        switch (picklistMapping.MappingType)
                        {
                            case SDK.Enums.PicklistMappingType.Automatic:

                                if (obj is OptionSetValue value)
                                {
                                    optionValue = value.Value;
                                }
                                else
                                {
                                    optionValue = Convert.ToInt32(obj.ToString());
                                }

                                
                                break;

                            case SDK.Enums.PicklistMappingType.Manual:

                                var key = obj.ToString();
                                if (obj is OptionSetValue v)
                                {
                                    key = v.Value.ToString();
                                }

                                var mapping = picklistMapping.Mapping.Where(t => t.Source == key).FirstOrDefault();
                                if (mapping == null && !String.IsNullOrEmpty(obj.ToString()))
                                {
                                    switch(picklistMapping.MappingNotFound)
                                    {
                                        case SDK.Enums.MappingNotFoundType.FailImport:
                                            throw new Exception("Could not map picklist " + dataMapping.Target + ": Mapping for source-value " + obj.ToString() + " could not be found!");
                                        
                                        case SDK.Enums.MappingNotFoundType.Ignore:
                                            // ignore
                                            break;

                                        case SDK.Enums.MappingNotFoundType.SetDefaultValue:
                                            optionValue = Convert.ToInt32(picklistMapping.DefaultValue);
                                            break;
                                    }
                                    
                                }
                                if (mapping != null)
                                {
                                    optionValue = Convert.ToInt32(mapping.Target);
                                }
                                break;
                        }

                        if(optionValue != -1)
                        {                            
                            if(dataMapping.Target == "statuscode")
                            {
                                StatusAttributeMetadata statusAttributeMetadata = attributeMetadata as StatusAttributeMetadata;
                                var statusOptionMetadata = statusAttributeMetadata.OptionSet.Options.Where(t=> t.Value == optionValue).First() as StatusOptionMetadata;
                                entity.Attributes.Add("statecode", new OptionSetValue(statusOptionMetadata.State.Value));
                            }
                            entity.Attributes.Add(dataMapping.Target, new OptionSetValue(optionValue));
                        }
                        break;

                    case AttributeTypeCode.Uniqueidentifier:
                        Guid id = new Guid(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, id);
                        break;

                    default:
                        throw new Exception("Could not convert attribute with type " + attributeMetadata.AttributeType.Value.ToString());
                }

            }
        }
    }
}
