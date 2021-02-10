using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class TableList
    {
        private void MainFunction()
        {
            OutputList = GetTables(false)
                .Where(e => string.IsNullOrEmpty(e.ColumnType))
                .Select(c => c.TableName)
                .ToList();
        }
    }
}