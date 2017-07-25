using IntegrationTool.Module.WriteToDynamicsCrm.UserControls.Relation;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using IntegrationTool.SDK.GenericControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls
{
    /// <summary>
    /// Interaction logic for RelationMapping.xaml
    /// </summary>
    public partial class RelationMapping : UserControl
    {
        private IOrganizationService service;
        private IDatastore dataObject;
        private EntityMetadata entityMetadata;
        private WriteToDynamicsCrmConfiguration Configuration { get; set; }

        public RelationMapping(IOrganizationService service, WriteToDynamicsCrmConfiguration configuration, IDatastore dataObject, EntityMetadata entityMetadata)
        {
            this.service = service;
            this.dataObject = dataObject;
            this.Configuration = configuration;
            this.entityMetadata = entityMetadata;
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            lbRelationAttributes.Items.Clear();
            List<string> relationAttributes = new List<string>();

            foreach(var relation in entityMetadata.ManyToOneRelationships.OrderBy(t=> t.ReferencingAttribute))
            {
                if (relationAttributes.Contains(relation.ReferencingAttribute))
                {
                    continue;
                }

                relationAttributes.Add(relation.ReferencingAttribute);
                var attributeMetadata = entityMetadata.Attributes.First(t => t.LogicalName == relation.ReferencingAttribute);
                
                switch(attributeMetadata.AttributeType.Value)
                {
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Owner:
                    case AttributeTypeCode.Customer:
                        ListBoxItem listBoxItem = new ListBoxItem()
                        {
                            Content = relation.ReferencingAttribute,
                            ToolTip = attributeMetadata.DisplayName.LocalizedLabels.Count == 0 ? "" : attributeMetadata.DisplayName.LocalizedLabels[0].Label
                        };
                        if (Configuration.RelationMapping.Count(t => t.LogicalName == relation.ReferencingAttribute) > 0)
                        {
                            listBoxItem.Background = new SolidColorBrush(Color.FromRgb(58, 162, 230));
                            listBoxItem.Foreground = Brushes.White; 
                        }
                        lbRelationAttributes.Items.Add(listBoxItem);
                        break;
                }                
            }
        }

        private void lbRelationAttributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem listboxItem = lbRelationAttributes.SelectedItem as ListBoxItem;
            if(listboxItem == null)
            {
                RelationMappingContent.Content = null;
                return;
            }

            var relations = entityMetadata.ManyToOneRelationships.Where(t => t.ReferencingAttribute == listboxItem.Content.ToString());
            IEnumerable<SDK.RelationMapping> relationMappings = Configuration.RelationMapping.Where(t => t.LogicalName == relations.First().ReferencingAttribute);
            
            // Is it a new relationmapping?
            if (relationMappings.Count() == 0 || relations.Count() > 1)
            {
                Relation.RelationMappingCreation relationMappingCreation = new Relation.RelationMappingCreation(relationMappings.ToList(), relations.ToList());
                relationMappingCreation.NewRelationCreated += relationMappingCreation_NewRelationCreated;
                RelationMappingContent.Content = relationMappingCreation;
            }
            else
            {
                OneToManyRelationshipMetadata relationShip = relations.First();
                if(relationShip.ReferencingAttribute == "ownerid")
                {
                    relationShip.ReferencedEntity = relationMappings.First().EntityName;
                }
                ShowRelationMapping(relationShip);
            }
        }

        void relationMappingCreation_NewRelationCreated(object sender, EventArgs e)
        {
            ShowRelationMapping(sender as OneToManyRelationshipMetadata);
        }

        private void ShowRelationMapping(OneToManyRelationshipMetadata relationMetadata)
        {
            RelationMappingContent.Content = new LoadingControl();
            BackgroundWorker bgwRelationKeyMappingWorker = new BackgroundWorker();
            bgwRelationKeyMappingWorker.DoWork += bgwRelationKeyMappingWorker_DoWork;
            bgwRelationKeyMappingWorker.RunWorkerCompleted += bgwRelationKeyMappingWorker_RunWorkerCompleted;
            bgwRelationKeyMappingWorker.RunWorkerAsync(relationMetadata);
        }

        void bgwRelationKeyMappingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OneToManyRelationshipMetadata relationMetadata = ((object[])e.Result)[0] as OneToManyRelationshipMetadata;
            EntityMetadata entityMetadata = ((object[])e.Result)[1] as EntityMetadata;
            
            SDK.RelationMapping relationMapping = Configuration.RelationMapping.Where(t => t.LogicalName == relationMetadata.ReferencingAttribute && t.EntityName == relationMetadata.ReferencedEntity).FirstOrDefault();
            if(relationMapping == null)
            {
                relationMapping = new SDK.RelationMapping() { LogicalName = relationMetadata.ReferencingAttribute, EntityName = relationMetadata.ReferencedEntity };
                Configuration.RelationMapping.Add(relationMapping);
            }

            List<NameDisplayName> sourceList = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetAllAttributesOfEntity(entityMetadata);

            Relation.RelationMappingKeyMapping relationMappingKeyMapping = new Relation.RelationMappingKeyMapping(relationMapping, sourceList, targetList);
            relationMappingKeyMapping.DeleteRelationMapping += relationMappingKeyMapping_DeleteRelationMapping;
            RelationMappingContent.Content = RelationMappingContent.Content = relationMappingKeyMapping;
        }

        void relationMappingKeyMapping_DeleteRelationMapping(object sender, EventArgs e)
        {
            var relationMapping = sender as IntegrationTool.Module.WriteToDynamicsCrm.SDK.RelationMapping;
            this.Configuration.RelationMapping.Remove(relationMapping);

            Initialize();
        }

        void bgwRelationKeyMappingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            OneToManyRelationshipMetadata relationShipMetadata = e.Argument as OneToManyRelationshipMetadata;
            EntityMetadata entityMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, relationShipMetadata.ReferencedEntity);

            e.Result = new object [] {relationShipMetadata, entityMetadata };
        }
    }
}
