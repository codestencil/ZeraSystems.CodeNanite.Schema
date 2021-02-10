using System.Collections.Generic;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class ExpandColumnsHorizontally
    {
        private void MainFunction()
        {
            AppendText();
            var lines = new List<string>();
            var columns = GetColumns(Input);
            var line = string.Empty;
            for (var i = 0; i < columns.Count; i++)
            {
                line += columns[i].ColumnName;
                if (i != (columns.Count - 1))
                    line += ", ";

                if (line.Length >= 100 || i == columns.Count - 1)
                {
                    lines.Add(line);
                    line = string.Empty;
                }
            }

            for (var j = 0; j < lines.Count; j++)
            {
                var item = lines[j];
                if (j == lines.Count - 1)
                {
                    AppendText(item.ToString(), "");
                    continue;
                }

                AppendText(item.ToString());
            }
        }
    }
}