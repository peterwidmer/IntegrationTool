﻿using IntegrationTool.Module.WriteToMySQL.UserControls;
using IntegrationTool.Module.WriteToMySQL.UserControls.Mapping;
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

namespace IntegrationTool.Module.WriteToMySQL
{
    /// <summary>
    /// Interaction logic for ConfigurationContent.xaml
    /// </summary>
    public partial class ConfigurationContent : UserControl
    {
        public ConfigurationContent(AttributeMapping attributeMapping, ImportSettings importSettings, RelationMapping relationMapping)
        {
            InitializeComponent();
            MappingTab.Content = attributeMapping;
            ImportSettingsTab.Content = importSettings;
            RelationmappingTab.Content = relationMapping;
        }
    }
}
