using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ZeraSystems.CodeNanite.Expansion;

//using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateModel
    {
        #region Fields
        private readonly string _public = "public ";
        private readonly string _getSet = " { get; set; }";
        private string _className;
        private string _table;
        private List<ISchemaItem> _columns;
        private List<ISchemaItem> _navProperties;
        private List<ISchemaItem> _selfRefColumns;
        private List<ISchemaItem> _foreignKeys;
        private bool _preserveTableName;
        #endregion
        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _table = Singularize(Input,_preserveTableName);
            _columns = GetColumns(Input, false); 
            _navProperties = GetNavProperties(Input);
            _foreignKeys = GetForeignKeysInTable(Input)
                .Where(k=>k.TableName != k.RelatedTable).ToList();
            _selfRefColumns = GetSelfJoinColumns(Input);
            _className = Singularize(Input,_preserveTableName) + GetExpansionString("MODEL_SUFFIX") + " ";

            AppendText();
            AppendText(_public + "partial class " + _className, 4, string.Empty);
            AppendText("{", 4);

            //Constructor
            if (_navProperties.Any(e => e.TableName!=_table))
                AppendText(GetConstructor(), string.Empty); 

            AppendText(GetColumns(), string.Empty); // This is not to allow line feed
            AppendText("}", 4, string.Empty);
        }

        List<string> _tableList = new List<string>();
        private string GetConstructor(int indent=8)
        {
            BuildSnippet(null);
            BuildSnippet(_public+_table+"()", indent);
            BuildSnippet("{", indent);

            _tableList.Clear();
            foreach (var item in _navProperties)
            {
                if (item.TableName == _table) continue;
                if (_tableList.Contains(item.TableName)) continue;
                BuildSnippet(Pluralize(item.TableName,_preserveTableName) + " = new HashSet<" + item.TableName + ">();", indent+4);
                GetSelfRefVanProperty(item);
                _tableList.Add(item.TableName);
            }
            BuildSnippet("}", indent);
            BuildSnippet("");
            return BuildSnippet();


            void GetSelfRefVanProperty(ISchemaItem item)
            {
                if (_selfRefColumns.Any())
                {
                    foreach (var col in _selfRefColumns)
                        BuildSnippet(col.SelfRefNavProperty() + " = new HashSet<" + _table + ">();", indent + 4);
                }
            }
        }

        private string GetColumns()
        {
            BuildSnippet(null);
            foreach (var item in _columns)
                BuildSnippet(_public + item.ColumnType + GetNullSign(item) + " " + item.ColumnName + _getSet);

            BuildSnippet("");
            GetNavProperties();
            return BuildSnippet();
        }

        private void GetNavProperties()
        {
            _tableList.Clear();
            foreach (var item in _navProperties)
            {
                if (item.TableName == item.RelatedTable)
                {
                    BuildSnippet(_public + "virtual " + item.TableName + " " + item.SelfReferenceColumn() + _getSet);
                    BuildSnippet(_public + "virtual ICollection<" + item.TableName + "> " + item.SelfRefNavProperty() + _getSet);
                }
                else
                {
                    if (_foreignKeys.Any())
                    {
                        //loop FKs
                        foreach (var key in _foreignKeys)
                            if (key.TableName != key.RelatedTable)
                                BuildSnippet(_public + "virtual " + key.RelatedTable + " " + CreateTablePropertyName(key) + _getSet);
                        _foreignKeys.Clear();
                    }
                    if (_tableList.Contains(item.TableName)) continue;
                    if (item.TableName != _table)
                        BuildSnippet(_public + "virtual ICollection<" + item.TableName + "> " + Pluralize(item.TableName,_preserveTableName) + _getSet);
                }
                _tableList.Add(item.TableName);

            }
        }



        private string GetTableString(string table)
        {
            return GetExpansionString("MODEL_PREFIX") + table + GetExpansionString("MODEL_SUFFIX");
        }
    }
}