using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DbRecord
    {
        public string TableName { get; set; }
        public List<KeyValuePair<string, object>> Values { get; set; }

        public DbRecord(string tableName)
        {
            this.TableName = tableName;
            Values = new List<KeyValuePair<string, object>>();
        }
    }
}
