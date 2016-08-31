using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntegrationTool.SDK.Diagram
{
    public class DiagramDeserializer
    {
        public BlockingCollection<DesignerItemBase> DesignerItems { get; set; }
        public BlockingCollection<ConnectionBase> Connections { get; set; }

        public DiagramDeserializer(List<ModuleDescription> moduleDescriptions, XElement root)
        {
            DesignerItems = new BlockingCollection<DesignerItemBase>();
            Connections = new BlockingCollection<ConnectionBase>();

            DeserializeDesignerItems(moduleDescriptions, root);
            DeserializeConnections(root);
        }

        private void DeserializeDesignerItems(List<ModuleDescription> moduleDescriptions, XElement root)
        {
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                DesignerItemBase item = DeserializeDesignerItem(moduleDescriptions, itemXML);
                item.State = ItemState.NotExecuted;
                DesignerItems.Add(item);
            }
        }

        private void DeserializeConnections(XElement root)
        {
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                ConnectionBase connection = DeserializeConnection(connectionXML);
                Connections.Add(connection);
            }
        }

        private DesignerItemBase DeserializeDesignerItem(List<ModuleDescription> moduleDescriptions, XElement itemXML)
        {
            DesignerItemBase item = new DesignerItemBase();
            item.ID = new Guid(itemXML.Element("ID").Value);
            item.ItemLabel = itemXML.Element("ItemLabel") == null ? "" : itemXML.Element("ItemLabel").Value;
            item.ModuleDescription = moduleDescriptions.Where(t => t.ModuleType.Name == itemXML.Element("ModuleDescription").Value).FirstOrDefault();
            return item;
        }

        private ConnectionBase DeserializeConnection(XElement connectionXML)
        {
            ConnectionBase connectionBase = new ConnectionBase();
            connectionBase.SourceID = new Guid(connectionXML.Element("SourceID").Value);
            connectionBase.SinkID = new Guid(connectionXML.Element("SinkID").Value);
            if (connectionXML.Element("ConnectionType") != null)
            {
                connectionBase.ConnectionType = (ConnectorType)Enum.Parse(typeof(ConnectorType), connectionXML.Element("ConnectionType").Value);
            }
            else
            {
                connectionBase.ConnectionType = ConnectorType.Default;
            }

            return connectionBase;
        }
    }
}
