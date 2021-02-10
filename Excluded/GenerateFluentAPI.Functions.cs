using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateFluentAPI
    {
        #region Fields

        private string _table;
        private List<ISchemaItem> _columns;
        private ISchemaItem _tableObject;
        private bool _preserveTableName ;
        public enum Reverse
        {
            WithRequired,
            WithOptional
        }

        #endregion Fields

        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _table = GetTable(Input, false);
            _tableObject = GetTableObject(Input);
            _columns = GetColumns(Input, false);
            //_columns = GetColumnsAndNavigation(Input, false);
            if (_columns == null)
                return;

            GenerateFluentApi();
        }

        private void GenerateFluentApi()
        {
            var indent = 8;
            AppendText();
            SetTableName(indent);
            SetPrimaryKey(indent);
            SetHasMany(indent);
        }

        private void SetTableName(int indent)
        {
            if (_tableObject != null)
                AppendText(Indent(indent) + "ToTable(" + _tableObject.OriginalName.AddQuotes() + ");");
        }

        private void SetPrimaryKey(int indent)
        {
            var cols = _columns
                .Where(c => c.IsPrimaryKey).ToList();

            if (cols.Count() > 1)
                SetCompositeKeys(cols, 8);
            else if (cols.Count == 1)
            {
                var col = cols.FirstOrDefault();
                if (col != null) AppendText(Indent(indent) + "HasKey(t => t." + col.ColumnName + ");");
            }
            else if (cols.Count == 0)
            {
                SetAnotherKey(8);
            }
        }

        private void SetAnotherKey(int indent)
        {
            var col = _columns.FirstOrDefault(c => c.IsForeignKey);
            if (col != null) AppendText(Indent(indent) + "HasKey(t => t." + col.ColumnName + ");");
        }

        private void SetCompositeKeys(IReadOnlyList<ISchemaItem> cols, int indent)
        {
            const string left = "HasKey(t => new { ";
            const string right = "});";
            var keys = string.Empty;
            for (var index = 0; index < cols.Count; index++)
            {
                var col = cols[index];
                keys += "t." + col.ColumnName.Comma(index + 1, cols.Count);
            }
            AppendText(Indent(indent) + left + keys + right);
        }

        private void SetHasMany(int indent)
        {
            const string left = "HasMany(t => t.";
            var tables = GetOne2ManyTable(_table);
            if (tables != null)
            {
                var hasString = string.Empty;
                foreach (var table in tables)
                {
                    var tableno = string.Empty;     //will be used to add "1" to the end of self-referencing tables
                    if (table.TableName == table.RelatedTable) //i.e. self-referencing table not implemented
                        continue;

                    //if (table.TableName == table.RelatedTable) //i.e. self-referencing table
                    //    tableno = "1";
                    hasString +=
                        Indent(indent) + left + Pluralize(table.TableName,_preserveTableName) + tableno + ")".AddCarriage() +
                                                                          Indent(indent + 4) + ReverseDirection(Reverse.WithRequired, table.TableName, _table, tableno) + ";".AddCarriage();
                }

                AppendText(hasString);
            }
        }

        private string ReverseDirection(Reverse direction, string table, string related, string tableno)
        {
            if (!IsInListOfTables(_table))
                return string.Empty;

            var text = "." + direction + "(t => t." + _table + tableno + ")";
            var fkColumn = GetForeignKeyColumn(table, related);
            if (fkColumn != null)
                text += ".HasForeignKey(t => t." + fkColumn + ")";
            return text;
        }

        //https://msdn.microsoft.com/en-us/data/jj591617#1.2
        //https://practiceaspnet.wordpress.com/2015/10/19/configuring-one-to-many-relationships-with-fluent-api/
    }
}