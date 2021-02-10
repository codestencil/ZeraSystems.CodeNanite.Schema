using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class TableListSLowerCase
    {
        private void MainFunction()
        {
            OutputList = GetTables(false)
                .Where(e => string.IsNullOrEmpty(e.ColumnType))
                .Select(c => Pluralize(c.TableName, PreserveTableName()).ToLower() )
                .ToList();
        }
    }
}