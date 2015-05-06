using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class StatusHelper
    {
        public static bool MustShowProgress(int index, int totalRecords)
        {
            index++;
            if(index <= 10)
            {
                return true;   
            }

            if(index > 10 && index <= 100 && index % 10 == 0)
            {
                return true;
            }

            if(index > 100 && index % 100 == 0)
            {
                return true;
            }

            if(index == (totalRecords))
            {
                return true;
            }

            return false;
        }
    }
}
