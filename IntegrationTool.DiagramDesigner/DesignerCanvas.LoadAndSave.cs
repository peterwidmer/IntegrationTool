using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Collections.Concurrent;
using IntegrationTool.Flowmanagement;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Diagram;

namespace IntegrationTool.DiagramDesigner
{
    public partial class DesignerCanvas
    {
        public XElement SaveDiagramToXElement()
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            return root;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                       from item in designerItems
                                       let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                       select new XElement("DesignerItem",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("ID", item.ID),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("IsGroup", item.IsGroup),
                                                  new XElement("ParentID", item.ParentID),
                                                  //new XElement("Content", contentXaml),
                                                  new XElement("ModuleDescription", item.ModuleDescription.ModuleType.Name),
                                                  new XElement("ItemLabel", item.ItemLabel)
                                              )
                                   );

            return serializedItems;
        }

        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("SourceArrowSymbol", connection.SourceArrowSymbol),
                                      new XElement("SinkArrowSymbol", connection.SinkArrowSymbol),
                                      new XElement("zIndex", Canvas.GetZIndex(connection)),
                                      new XElement("ConnectionType", connection.Type)                                     
                                  ));

            return serializedConnections;
        }

        public void LoadDiagramFromXElement(XElement root)
        {
            if (root == null)
                return;

            this.Children.Clear();
            this.SelectionService.ClearSelection();

            Style itemStyle = (Style)FindResource("Process");
            Style itemControlTemplateStyle = (Style)FindResource("Process_DragThumb");

            List<DesignerItem> designerItems = LoadDiagramDesignerItems(root, this.ModuleDescriptions);
            foreach (DesignerItem designerItem in designerItems)
            {
                designerItem.Content = GetContentItem(itemStyle, itemControlTemplateStyle, designerItem.ModuleDescription);
                this.Children.Add(designerItem);
                SetConnectorDecoratorTemplate(designerItem);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                if (connectionXML.Element("ConnectionType") != null)
                {
                    connection.Type = (ConnectorType)Enum.Parse(typeof(ConnectorType), connectionXML.Element("ConnectionType").Value);
                }
                else
                {
                    connection.Type = ConnectorType.Default;
                }
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                this.Children.Add(connection);
            }
        }

        public static List<DesignerItem> LoadDiagramDesignerItems(XElement root, List<ModuleDescription> moduleDescriptions)
        {
            List<DesignerItem> designerItems = new List<DesignerItem>();
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(moduleDescriptions, itemXML, id, 0, 0);
                
                designerItems.Add(item);
            }
            return designerItems;
        }

        private static DesignerItem DeserializeDesignerItem(List<ModuleDescription> moduleDescriptions, XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            var itemLabel = itemXML.Element("ItemLabel");
            item.ItemLabel = itemLabel == null ? "" : itemLabel.Value;
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            
            item.ModuleDescription = moduleDescriptions.Where(t => t.ModuleType.Name == itemXML.Element("ModuleDescription").Value).FirstOrDefault();
            
            return item;
        }

        private static object GetContentItem(Style itemStyle, Style itemControlTemplateStyle, ModuleDescription module)
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path { ToolTip = module.Attributes.DisplayName };
            path.Style = itemStyle;
            var processElement = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path));
            processElement.SetValue(StyleProperty, itemControlTemplateStyle);
            ControlTemplate controlTemplate = new ControlTemplate();
            controlTemplate.VisualTree = processElement;
            path.SetValue(DesignerItem.DragThumbTemplateProperty, controlTemplate);
            path.Tag = module;
            
            return path;
        }
    }
}
