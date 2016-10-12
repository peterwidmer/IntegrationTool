using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseMetadataProviders
{
    public class MsSqlMetadataProvider: IDatabaseMetadataProvider
    {
        private IConnection connection;


        public MsSqlMetadataProvider(IConnection connection)
        {
            this.connection = connection;
        }

        public List<DbMetadataTable> DatabaseTables { get; set; }

        public void Initialize()
        {
            this.DatabaseTables = GetDatabaseTables();
        }

        private List<DbMetadataTable> GetDatabaseTables()
        {
            DataTable dt = null;
            List<DbMetadataTable> tables = new List<DbMetadataTable>();

            // Todo - get list of all tables

            return tables;
        }
    }
}
