using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;

//using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateLookups
    {
        #region Fields
        private readonly string _public = "public ";
        private readonly string _getSet = " { get; set; }";
        private string _table;
        private List<ISchemaItem> _foreignKeys;
        private bool _preserveTableName;
        #endregion

        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _table = GetTable(Singularize(Input,_preserveTableName));
            _foreignKeys = GetForeignKeysInTable(_table);
            //_foreignKeys = GetNavProperties(_table);

            AppendText();
            if (!_foreignKeys.Any())
            {
                AppendText(Indent(8)+"// Add your code here if you are creating lookup manually.");
                return;
            }

            AppendText(GetEachLookup(), string.Empty); // This is not to allow line feed
        }

        private string GetEachLookup()
        {
            BuildSnippet(null);
            foreach (var item in _foreignKeys)
            {
                var sL = item.RelatedTable + "Sl";
                //var selectList = "Select" + item.RelatedTable;
                var query = Pluralize(item.RelatedTable,_preserveTableName).ToLower()+"Query";
                var selectedTable = "selected" + item.RelatedTable;
                BuildSnippet(_public + "SelectList " + sL  + _getSet, 8);
                BuildSnippet("");
                BuildSnippet(_public + " void " + "Populate" + item.RelatedTable + "Lookup(" + GetDbContext() +
                             " context, object " + selectedTable + " = null)", 8);
                BuildSnippet("{", 8);

                BuildSnippet("var " + query +" = from q in context."+Singularize(_table,_preserveTableName)+" orderby q."+GetTableLabel(_table)+" select q;", 12);
                BuildSnippet("");
                BuildSnippet(sL + " = new SelectList(" + query + ".AsNoTracking(), " +
                             item.ColumnName.AddQuotes() + ", " + GetTableLabel(_table).AddQuotes() + ", " +
                             selectedTable+");", 12);
                BuildSnippet("}", 8);
                BuildSnippet("");
            }
            return BuildSnippet();
        }

    }
}