using Microsoft.Xrm.Sdk.Metadata;
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

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls.Relation
{
    /// <summary>
    /// Interaction logic for RelationMappingCreation.xaml
    /// </summary>
    public partial class RelationMappingCreation : UserControl
    {
        private List<OneToManyRelationshipMetadata> relationsMetadata;
        private List<SDK.RelationMapping> relationMappings;

        public event EventHandler NewRelationCreated;

        public RelationMappingCreation(List<SDK.RelationMapping> relationMappings, List<OneToManyRelationshipMetadata> relationsMetadata)
        {
            this.relationMappings = relationMappings.OrderBy(t=>t.EntityName).ToList();
            this.relationsMetadata = relationsMetadata;
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            foreach(var relMapping in relationMappings)
            {
                lbExistingMappings.Items.Add(new ListBoxItem() { Content = relMapping.EntityName, Tag = relMapping });
            }

            foreach(var relation in relationsMetadata.OrderBy(t=> t.ReferencedEntity))
            {
                if (relation.ReferencedEntity == "owner")
                {
                    ddEntities.Items.Add(new ComboBoxItem() { Content = "systemuser", Tag = relation });
                    ddEntities.Items.Add(new ComboBoxItem() { Content = "team", Tag = relation });
                }
                else
                {
                    ddEntities.Items.Add(new ComboBoxItem() { Content = relation.ReferencedEntity, Tag = relation });
                }
            }
            if(ddEntities.Items.Count == 1)
            {
                ddEntities.SelectedIndex = 0;
                ddEntities.IsEnabled = false;
            }
        }

        private void btnCreateRelation_Click(object sender, RoutedEventArgs e)
        {
            if (ddEntities.SelectedItem != null)
            {
                OneToManyRelationshipMetadata relationMetadata = ((ComboBoxItem)ddEntities.SelectedItem).Tag as OneToManyRelationshipMetadata;
                relationMetadata.ReferencedEntity = ((ComboBoxItem)ddEntities.SelectedItem).Content.ToString();
                SetRelationChosen(relationMetadata);
            }
        }

        private void lbExistingMappings_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbExistingMappings.SelectedItem != null)
            {
                SDK.RelationMapping relMapping = ((ListBoxItem)lbExistingMappings.SelectedItem).Tag as SDK.RelationMapping;
                OneToManyRelationshipMetadata relMetadata = relationsMetadata.Where(t => t.ReferencingAttribute == relMapping.LogicalName && t.ReferencedEntity == relMapping.EntityName).First();
                SetRelationChosen(relMetadata);
            }
        }

        private void SetRelationChosen(OneToManyRelationshipMetadata selectedRelation)
        {
            if (NewRelationCreated != null)
            {
                NewRelationCreated(selectedRelation, new EventArgs());
            }
        }

        
    }
}
