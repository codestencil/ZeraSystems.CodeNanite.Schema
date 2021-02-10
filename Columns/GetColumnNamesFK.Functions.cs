using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GetColumnNamesFK
    {
        private string _table;
        private void MainFunction()
        {
            _table = GetTable(Input);
            OutputList = GetColumns(_table)
                .Where(e => !string.IsNullOrEmpty(e.ColumnType))
                .Where(t => t.IsChecked)
                .Where(t => t.IsForeignKey)
                .Select(s=>s.ColumnName)
                .ToList();
        }
    }
}