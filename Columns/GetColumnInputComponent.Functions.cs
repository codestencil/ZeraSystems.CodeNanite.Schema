using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GetColumnInputComponent
    {
        private void MainFunction()
        {
            var columnTypes = GetColumns(Input)
                .Where(e => !string.IsNullOrEmpty(e.ColumnType))
                .Where(t => t.IsChecked)
                .Where(t => !t.IsPrimaryKey)
                .Select(s=>s.ColumnType)
                .ToList();

            OutputList = new List<string>();
            foreach (var item in columnTypes)
            {
                var inputType = "InputNumber";
                if (item == "string" || item == "char" || item == "Guid")
                    inputType = "InputText";
                else if (inputType == "bool")
                    inputType = "InputCheckbox";
                else if (inputType == "DateTime")
                    inputType = "InputDate";
                OutputList.Add(inputType);
            }
        }
    }
}

//DataType.Add(new DataType { Name = "byte" });
//DataType.Add(new DataType { Name = "sbyte" });
//DataType.Add(new DataType { Name = "char" });
//DataType.Add(new DataType { Name = "decimal" });
//DataType.Add(new DataType { Name = "double" });
//DataType.Add(new DataType { Name = "float" });
//DataType.Add(new DataType { Name = "int" });
//DataType.Add(new DataType { Name = "uint" });
//DataType.Add(new DataType { Name = "long" });
//DataType.Add(new DataType { Name = "ulong" });
//DataType.Add(new DataType { Name = "object" });
//DataType.Add(new DataType { Name = "short" });
//DataType.Add(new DataType { Name = "ushort" });
//DataType.Add(new DataType { Name = "DateTime" });
//DataType.Add(new DataType { Name = "Guid" });
