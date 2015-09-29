using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace IntegrationTool.Module.ConnectToExcel.UserControls
{
    /// <summary>
    /// Interaction logic for ExistingFileAndSheetControl.xaml
    /// </summary>
    public partial class ExistingFileAndSheetControl : UserControl
    {
        private ConnectToExcelConfiguration configuration;

        public ExistingFileAndSheetControl(ConnectToExcelConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;

            if (String.IsNullOrEmpty(configuration.FilePath) == false)
            {
                InitializeConnection(configuration.FilePath);
            }

            if (String.IsNullOrEmpty(configuration.SheetName) == false)
            {
                ddSheetsInWorkbook.SelectedItem = ddSheetsInWorkbook.Items.Cast<string>().
                                                            Where(t => t == configuration.SheetName).FirstOrDefault();
            }
        }

        private void btnOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileBrowserDialog = new System.Windows.Forms.OpenFileDialog();
            if (fileBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InitializeConnection(fileBrowserDialog.FileName);
            }
        }

        private void InitializeConnection(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Exists == false)
            {
                MessageBox.Show("Could not load selected excelsheet!");
                return;
            }

            try
            {
                tbExcelsheetFilePath.Text = fileName;

                ExcelPackage pck = new ExcelPackage(file);
                ddSheetsInWorkbook.Items.Clear();
                foreach (var sheet in pck.Workbook.Worksheets)
                {
                    ddSheetsInWorkbook.Items.Add(sheet.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when loading excelsheet: " + ex.Message);
            }
        }

        private void ddSheetsInWorkbook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.configuration.SheetName = ddSheetsInWorkbook.SelectedItem as string;
        }
    }
}
