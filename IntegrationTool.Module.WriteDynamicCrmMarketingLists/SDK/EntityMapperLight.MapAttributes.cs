using IntegrationTool.Module.CrmWrapper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public partial class EntityMapperLight
    {
        public void MapAttributes(Entity entity, object[] data)
        {
            foreach (var dataMapping in this.mappings)
            {
                AttributeMetadata attributeMetadata = this.attributeMetadataDictionary[dataMapping.Target];
                object obj = data[this.columnMetadataDictionary[dataMapping.Source].ColumnIndex];

                if (obj == null)
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
                        throw new NotImplementedException("Customermappings are not supported in marketinglists right now!");

                    case AttributeTypeCode.DateTime:
                        if (obj.GetType() == typeof(DateTime))
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
                        decimal decimalValue = Convert.ToDecimal(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, decimalValue);
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
                    case AttributeTypeCode.Lookup:
                        throw new NotImplementedException("Loojups are not supported in marketinglists right now!");

                    case AttributeTypeCode.Money:
                        decimal moneyValue = Convert.ToDecimal(obj.ToString());
                        entity.Attributes.Add(dataMapping.Target, new Money(moneyValue));
                        break;

                    case AttributeTypeCode.Memo:
                    case AttributeTypeCode.String:
                        string stringValue = obj.ToString();
                        entity.Attributes.Add(dataMapping.Target, stringValue);
                        break;

                    case AttributeTypeCode.Status:
                    case AttributeTypeCode.Picklist:
                        throw new NotImplementedException("Picklists are not supported in marketinglists right now!");

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
