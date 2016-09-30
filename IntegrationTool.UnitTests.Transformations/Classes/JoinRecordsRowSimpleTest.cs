using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Transformations.Classes
{
    public class JoinRecordsRowSimpleTest
    {
        public int ? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int ? PersonId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
