using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.Crm2013Wrapper;
using IntegrationTool.SDK;
using IntegrationTool.UnitTests.Targets.Properties;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Targets
{
    public class CrmDefaultUnits
    {
        public Guid DefaultUnitGroupId;
        public Guid PrimaryUnitId;
    }

    public class Test_Helpers
    {
        public static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");

        public static IConnection GetDynamicsCrmConnection()
        {
            ConnectToDynamicsCrmConfiguration configuration = new ConnectToDynamicsCrmConfiguration();
            configuration.ConnectionString = Settings.Default.CrmConnectionString;

            return new ConnectToDynamicsCrm() { Configuration = configuration };
        }

        public static CrmDefaultUnits GetDefaultUnitGroup(IOrganizationService service)
        {
            var defaultUnits = new CrmDefaultUnits();

            var defaultUnitsEntities = Crm2013Wrapper.RetrieveMultiple(
                service, "uomschedule",
                new Microsoft.Xrm.Sdk.Query.ColumnSet(new string[] { "uomscheduleid" }),
                new Microsoft.Xrm.Sdk.Query.ConditionExpression("name", ConditionOperator.Equal, "Default Unit"));

            Guid defaultUnitGroupId = (Guid)defaultUnitsEntities[0]["uomscheduleid"];

            var primaryUnitEntities = Crm2013Wrapper.RetrieveMultiple(
                service, "uom",
                new Microsoft.Xrm.Sdk.Query.ColumnSet(new string[] { "uomid" }),
                new Microsoft.Xrm.Sdk.Query.ConditionExpression("uomscheduleid", ConditionOperator.Equal, defaultUnitGroupId));

            Guid primaryUnitId = (Guid)primaryUnitEntities[0]["uomid"];

            return new CrmDefaultUnits() { DefaultUnitGroupId = defaultUnitGroupId, PrimaryUnitId = primaryUnitId };
        }

        public static void ReportProgressMethod(SimpleProgressReport progress) { }   
    }
}
