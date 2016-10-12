using IntegrationTool.DBAccess;
using IntegrationTool.Module.WriteToMySql;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrationTool.Module.WriteToMySQL.UserControls.Mapping
{
    /// <summary>
    /// Interaction logic for AttributeMapping.xaml
    /// </summary>
    public partial class AttributeMapping : UserControl
    {
        private IDatastore dataObject;
        private DbMetadataTable selectedTable;

        public AttributeMapping(WriteToMySQLConfiguration configuration, IDatastore dataObject, DbMetadataTable selectedTable)
        {
            this.dataObject = dataObject;
            this.selectedTable = selectedTable;

            InitializeComponent();
            InitializeMappingControl(configuration.Mapping);
        }

        private void InitializeMappingControl(List<DataMappingControl.DataMapping> mapping)
        {
            List<NameDisplayName> sourceColumns = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (var column in sourceColumns)
            {
                SourceTargetMapping.SourceList.Add(new ListViewItem() { Content = column.Name, ToolTip = column.DisplayName });
            }

            foreach (var column in selectedTable.Columns)
            {
                SourceTargetMapping.TargetList.Add(new ListViewItem() { Content = column.ColumnName, ToolTip = column.ColumnName });
            }

            SourceTargetMapping.Mapping = mapping;
        }
    }
}
