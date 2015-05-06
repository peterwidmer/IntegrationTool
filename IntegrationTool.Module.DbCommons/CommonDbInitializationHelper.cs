using IntegrationTool.DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.Module.DbCommons
{
    public class CommonDbInitializationHelper
    {
        public static void InitializeTargetTable(IDatabaseMetadataProvider metadataProvider, ComboBox targetTableComboBox, string configurationTargetTable)
        {
            foreach (var table in metadataProvider.DatabaseTables)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = table.TableName, ToolTip = table.TableName, Tag = table };
                targetTableComboBox.Items.Add(comboBoxItem);
                if (configurationTargetTable == table.TableName)
                {
                    targetTableComboBox.SelectedItem = comboBoxItem;
                }
            }

            targetTableComboBox.IsEnabled = true;
        }
    }
}
