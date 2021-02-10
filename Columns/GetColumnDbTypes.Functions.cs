using System.Collections.Generic;
using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GetColumnDbTypes
    {
        private void MainFunction()
        {
            OutputList = new List<string>();
             var list = GetColumns(Input)
                .Where(e => !string.IsNullOrEmpty(e.ColumnType))
                .Where(t => t.IsChecked)
                .Where(t => !t.IsPrimaryKey)
                .Select(s=>s.ColumnType)
                .ToList();

            foreach (var item in list)
            {
                //string dataType;
                switch (item)
                {
                    case "string":
                        OutputList.Add("DbType.String");
                        break;
                    case "int":
                        OutputList.Add("DbType.Int32");
                        break;
                    case "DateTime":
                        OutputList.Add("DbType.Date");
                        break;
                    case "bool":
                        OutputList.Add("DbType.Boolean");
                        break;
                    case "decimal":
                        OutputList.Add("DbType.Decimal");
                        break;
                    case "byte":
                        OutputList.Add("DbType.Binary");
                        break;
                    case "double":
                        OutputList.Add("DbType.Double");
                        break;
                    case "float":
                        OutputList.Add("DbType.Double");
                        break;
                    case "uint":
                        OutputList.Add("DbType.UInt32");
                        break;
                    case "short":
                        OutputList.Add("DbType.Int16");
                        break;
                    case "object":
                        OutputList.Add("DbType.Object");
                        break;
                    //case "sbyte":
                    //    OutputList.Add("DbType.Object";
                    //    break;
                    //case "char":
                    //    OutputList.Add("DbType.Object";
                    //    break;
                    //case "ushort":
                    //    OutputList.Add("DbType.Object";
                    //    break;
                    //default:
                    //    dataType = item;
                    //    break;
                }
                //OutputList.Add(dataType);
            }
        }
    }
}