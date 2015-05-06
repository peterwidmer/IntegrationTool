using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DataMappingControl
{
    public class DataHelper
    {

        ///// <summary>
        ///// Checks the mapping against the available data. The mappings which are not available in the source are returned in the returntable
        ///// </summary>
        ///// <param name="MappingTable"></param>
        ///// <param name="SourceData"></param>
        ///// <returns></returns>
        //public static DataTable CheckMappingTable(DataTable MappingTable, DataTable SourceData)
        //{
        //    DataTable dtReturnTable = GetMappingTable();

        //    if (MappingTable == null)
        //        return dtReturnTable;

        //    DataRow drReturn = null;
        //    for (int i = 0; i < MappingTable.Rows.Count; i++)
        //    {
        //        if (!SourceData.Columns.Contains(MappingTable.Rows[i]["Source"].ToString()))
        //        {
        //            drReturn = dtReturnTable.NewRow();
        //            drReturn = CopyRow(drReturn, MappingTable.Rows[i]);
        //            dtReturnTable.Rows.Add(drReturn);
        //            MappingTable.Rows.RemoveAt(i);
        //            i--;
        //        }
        //    }

        //    return dtReturnTable;
        //}

        public static DataRow CopyRow(DataRow drReturn, DataRow dr)
        {
            drReturn["Source"] = dr["Source"];
            drReturn["Target"] = dr["Target"];
            drReturn["pkKey"] = dr["pkKey"];
            drReturn["active"] = dr["active"];
            drReturn["AdditionalInfos"] = dr["AdditionalInfos"];
            if (dr.Table.Columns.Contains("sourceformat") && drReturn.Table.Columns.Contains("sourceformat"))
                drReturn["sourceformat"] = dr["sourceformat"];

            return drReturn;
        }
    }
}
