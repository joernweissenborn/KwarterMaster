using System.Collections.Generic;

namespace KwarterMaster
{
    public class TableInfo
    {
        public string[] Header { get; }
        public int[] ColumnSizes { get; set; }
        public
        List<string[]> Data { get; set; }

        public TableInfo(string[] header, int[] columnSizes)
        {
            Header = header;
            ColumnSizes = columnSizes;
            Data = new List<string[]>();
        }
    }
}