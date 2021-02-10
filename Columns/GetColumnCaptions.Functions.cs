using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GetColumnCaptions
    {
        private void MainFunction()
        {
            OutputList = GetColumns(Input)
                .Where(e => !string.IsNullOrEmpty(e.ColumnType))
                .Where(t => t.IsChecked)
                .Where(t => !t.IsPrimaryKey)
                .Select(s=>s.ColumnLabel)
                .ToList();
        }
    }
}