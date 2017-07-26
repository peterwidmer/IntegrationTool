using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public class EntityAttributeComparer
    {
        private Dictionary<string, AttributeMetadata> attributeMetadata;

        private Dictionary<AttributeTypeCode, Func<object, object, bool>> comparerFunctions = new Dictionary<AttributeTypeCode, Func<object, object, bool>>
        {
            { AttributeTypeCode.BigInt, ObjectsAreEqual },
            { AttributeTypeCode.Boolean, ObjectsAreEqual },
            { AttributeTypeCode.CalendarRules, UnknownTypes},
            { AttributeTypeCode.Customer, EntityReferencesAreEqual},
            { AttributeTypeCode.DateTime, ObjectsAreEqual},
            { AttributeTypeCode.Decimal, ObjectsAreEqual},
            { AttributeTypeCode.Double, ObjectsAreEqual},
            { AttributeTypeCode.EntityName, UnknownTypes},
            { AttributeTypeCode.Integer, ObjectsAreEqual},
            { AttributeTypeCode.Lookup, EntityReferencesAreEqual},
            { AttributeTypeCode.ManagedProperty, UnknownTypes},
            { AttributeTypeCode.Memo, ObjectsAreEqual},
            { AttributeTypeCode.Money, MoneysAreEqual},
            { AttributeTypeCode.Owner, EntityReferencesAreEqual},
            { AttributeTypeCode.PartyList, UnknownTypes},
            { AttributeTypeCode.Picklist, OptionSetsAreEqual},
            { AttributeTypeCode.State, OptionSetsAreEqual},
            { AttributeTypeCode.Status, OptionSetsAreEqual},
            { AttributeTypeCode.String, ObjectsAreEqual},
            { AttributeTypeCode.Uniqueidentifier, UnknownTypes},
            { AttributeTypeCode.Virtual, UnknownTypes},
        };

        public EntityAttributeComparer(Dictionary<string, AttributeMetadata> attributeMetadata)
        {
            this.attributeMetadata = attributeMetadata;
        }

        public bool AttributesAreEqual(string logicalAttributeName, object value1, object value2)
        {
            if (value1 == null && value2 == null) 
            { 
                return true; 
            }
            else if(value1 != null && value2 == null || value1 == null && value2 != null)
            {
                return false;
            }
            else
            {
                var attributeTypeCode = attributeMetadata[logicalAttributeName].AttributeType.Value;
                if(comparerFunctions.ContainsKey(attributeTypeCode))
                {
                    return comparerFunctions[attributeMetadata[logicalAttributeName].AttributeType.Value](value1, value2);
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool ObjectsAreEqual(object value1, object value2)
        {
            return value1.ToString() == value2.ToString();
        }

        public static bool UnknownTypes(object value1, object value2)
        { 
            return false;
        }

        public static bool EntityReferencesAreEqual(object value1, object value2)
        {
            return ((EntityReference)value1).Id == ((EntityReference)value2).Id;
        }

        public static bool MoneysAreEqual(object value1, object value2)
        {
            return ((Money)value1).Value == ((Money)value2).Value;
        }

        public static bool OptionSetsAreEqual(object value1, object value2)
        {
            return ((OptionSetValue)value1).Value == ((OptionSetValue)value2).Value;
        }

    }
}
