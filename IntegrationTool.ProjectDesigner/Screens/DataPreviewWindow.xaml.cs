using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace IntegrationTool.ProjectDesigner.Screens
{
    /// <summary>
    /// Interaction logic for DataPreviewWindow.xaml
    /// </summary>
    public partial class DataPreviewWindow : Window
    {
        public DataPreviewWindow(IDatastore datastore)
        {
            InitializeComponent();

            DataTable dt = new DataTable();
            foreach(var column in datastore.Metadata.Columns)
            {
                dt.Columns.Add(column.ColumnName);
            }

            for(int i=0; i< datastore.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int iCol = 0; iCol < dt.Columns.Count; iCol++)
                {
                    if(datastore[i][iCol] ==  null || datastore[i][iCol] == DBNull.Value)
                    {
                        dr[iCol] = "";
                    }
                    else
                    {
                        dr[iCol] = datastore[i][iCol].ToString();
                    }
                }
                dt.Rows.Add(dr);
            }

            this.DataPreviewGrid.DataContext = dt;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
