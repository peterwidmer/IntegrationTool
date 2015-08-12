using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTool.Module.XmlTransformation
{
    public class XmlTransformationConfiguration : TransformationConfiguration
    {
        public string TransformationXslt { get; set; }

        public XmlTransformationConfiguration()
        {
            if(String.IsNullOrEmpty(this.TransformationXslt))
            {
                this.TransformationXslt =
"<?xml version=\"1.0\"?>\n" +
"<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\n" +
"<xsl:output method=\"xml\" indent=\"yes\"/>\n\n" +
@"<!-- 
    Themplate for transformation to table
    Adjust this xslt to your needs
    Press 'Preview transformation' to see the output  
-->" + "\n" +
"<xsl:template match=\"/\">\n" +
@"<it_table>
    <it_row>
        <myfield>The Field</myfield>
        <another>The Other</another>
    </it_row>
</it_table>
</xsl:template>

</xsl:stylesheet>";

            }
        }
    }
}
