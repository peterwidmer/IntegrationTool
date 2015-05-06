using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DbMetadataTable
    {
        public string TableName { get; set; }
        public List<DbMetadataColumn> Columns { get; set; }

        public DbMetadataTable()
        {
            Columns = new List<DbMetadataColumn>();
        }
    }
}
