using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateFluentConfig
    {

        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _table = GetTable(Input, false);
            _tableObject = GetTableObject(Input, false);
            _columns = GetColumnsExCalculated(Input);  //GetColumns(Input, false, true);
            //GetColumns(_schemaItem, string table, bool onlyIsChecked = true, string excludeColumn = null, bool noCalculated = false)
            if (_columns == null)
                return;
            GenerateFluentApi();
        }

        private void GenerateFluentApi()
        {
            AppendText();

            if (_tableObject != null)
            {
                AppendText(Indent(indent) + FormatTableName());
                AppendText(Indent(indent + 4) + FormatPrimaryKey());
            }
            else
                return;

            foreach (var column in _columns)
            {
                //_total12M = false;
                AppendText(FormatIndex(column, 12));
                AppendText(Indent(8) + "entity.Property(e => e." + column.ColumnName + ")");
                AppendText(GetFluentProperties(column, 12));
            }
        }

        
        private string GetFluentProperties(ISchemaItem row, int indent)
        {
            var result = string.Empty;
            result += Indent(indent) + ".HasColumnName(" + row.ColumnName.AddQuotes() + ")".AddCarriage();
            result += Indent(indent) + ".HasColumnType(" + row.ColumnType.AddQuotes() + ")".AddCarriage();
            if (row.IsRequired)
                result += Indent(indent) + ".IsRequired()".AddCarriage();

            //if (!row.ConstraintName.IsBlank())
            //    result += Indent(indent) + ".ConstraintName(" + row.ConstraintName.AddQuotes() + ")";

            if (row.ColumnType == "string" && row.ColumnSize > 0)
                result += Indent(indent) + ".HasMaxLength(" + row.ColumnSize + ")";
            
            var lastChar = result.Substring(result.Length - 2);
            if (lastChar == Environment.NewLine)
                result = result.Remove(result.Length - 2, 2);

            result += ";".AddCarriage();

            //result += Indent(indent) + FormatOneToMany(row, indent);
            var formatOneToMany = FormatOneToMany(row, indent);
            if (!_total12M)
            {
                result += Indent(indent) + formatOneToMany;
                _total12M = true;
            }

            //result += Indent(indent) + FormatOneToOne(row, indent);
            return result;
        }

        private string FormatTableName()
        {
            var result = string.Empty;
            if (_tableObject != null)
                result = "entity.ToTable(" + _tableObject.OriginalName.AddQuotes() + ")";

            return result;
        }


        private string FormatPrimaryKey()
        {
            var result = string.Empty;
            var cols = _columns.Where(c => c.IsPrimaryKey).ToList();

            if (cols.Count() > 1)
                result = FormatCompositeKeys(cols);
            else if (cols.Count == 1)
            {
                var col = cols.FirstOrDefault();
                if (col != null)
                    result = ".HasKey(e => e." + col.ColumnName + ");".AddCarriage();
            }
            else if (cols.Count == 0)
            {
                var col = _columns.FirstOrDefault(c => c.IsForeignKey);
                if (col != null)
                    result = ".HasKey(e => e." + col.ColumnName + ");".AddCarriage();
            }
            return result;
        }

        private string FormatCompositeKeys(IReadOnlyList<ISchemaItem> cols)
        {
            var keys = string.Empty;
            for (var index = 0; index < cols.Count; index++)
            {
                var col = cols[index];
                keys += "e." + col.ColumnName.Comma(index + 1, cols.Count);
            }
            var result = ".HasKey(e => new { " + keys + "});".AddCarriage();
            return result; ;
        }

        private string FormatIndex(ISchemaItem row, int indent)
        {
            var result = string.Empty;
            if (!row.IndexName.IsBlank() && !row.IsPrimaryKey)
            {
                result += Environment.NewLine;
                result += Indent(indent-4) + "entity.HasIndex(e => e." + row.ColumnName + ")".AddCarriage();
                result += Indent(indent) + ".HasName(" + row.IndexName.AddQuotes() + ");".AddCarriage();
            }

            return result;
        }

        private string FormatOneToMany(ISchemaItem row, int indent)
        {
            var result = string.Empty;
            if ( ((row.IsForeignKey && !row.RelatedTable.IsBlank()) && !row.IsPrimaryKey) || 
                 (row.IsForeignKey && row.IsPrimaryKey)     //Support for intermediate table in many-to-many
                 )
            {
                var relatedTable = row.RelatedTable;
                if (row.RelatedTable == row.TableName)  //Indicating a table related to itself
                    relatedTable = row.ColumnName + NavigationLabel();
                result += Environment.NewLine;
                //result += Indent(indent-4) + "entity.HasOne(d => d." + relatedTable + ")".AddCarriage();

                if (row.RelatedTable != row.TableName)  //Indicating a table related to itself
                    result += Indent(indent - 4) + "entity.HasOne(d => d." + CreateTablePropertyName(row) + ")".AddCarriage();
                else
                    result += Indent(indent - 4) + "entity.HasOne(d => d." + row.SelfReferenceColumn() + ")".AddCarriage();

                if (row.RelatedTable == row.TableName)  //Indicating a table related to itself
                    result += Indent(indent) + ".WithMany(p => p." + row.SelfRefNavProperty() + ")".AddCarriage();
                else
                    result += Indent(indent) + ".WithMany(p => p." + Pluralize(_table,_preserveTableName) + ")".AddCarriage() ;

                result += Indent(indent) + ".HasForeignKey(d => d." + row.ColumnName + ")";

                //BuildSnippet(".OnDelete(DeleteBehavior.Restrict)");
                if (row.ConstraintName.IsBlank())
                    result += ";";
                else
                    result += "".AddCarriage() +Indent(indent) + ".HasConstraintName(" + row.ConstraintName.AddQuotes()+");";
            }
            return result;
            //CreateTablePropertyName(row)
        }

        private string FormatOneToOne(ISchemaItem row, int indent)
        {
            var result = string.Empty;
            if ((row.IsPrimaryKey && row.IsForeignKey && !row.RelatedTable.IsBlank()) )
            {
                var relatedTable = row.RelatedTable;
                if (row.RelatedTable == row.TableName)  //Indicating a table related to itself
                    relatedTable = row.ColumnName + NavigationLabel();

                result += Environment.NewLine;
                result += Indent(indent - 4) + "// OneToOne ("+ relatedTable+"<->"+_table + ")".AddCarriage();
                result += Indent(indent - 4) + "entity.HasOne(p => p." + relatedTable + ")".AddCarriage();
                result += Indent(indent) + ".WithOne(i => i." + _table + ")".AddCarriage();
                result += Indent(indent) + ".HasForeignKey<"+_table+">(b => b." + GetPrimaryKey(_table) + ")";
                
                if (row.ConstraintName.IsBlank())
                    result += ";";
                else
                    result += "".AddCarriage() + Indent(indent) + ".HasConstraintName(" + row.ConstraintName.AddQuotes() + ");";
            }
            return result;


        }


        //private void GetColumnType(ISchemaItem row)
        //{
        //    switch (row.ColumnType)
        //    {
        //        case "bool":
        //            break;
        //        case "byte":
        //            break;
        //        case "sbyte":
        //            break;
        //        case "char":
        //            break;
        //        case "decimal":
        //            break;
        //        case "double":
        //            break;
        //        case "float":
        //            break;
        //        case "int":
        //            break;
        //        case "uint":
        //            break;
        //        case "long":
        //            break;
        //        case "ulong":
        //            break;
        //        case "object":
        //            break;
        //        case "short":
        //            break;
        //        case "ushort":
        //            break;
        //        case "DateTime":
        //            break;
        //        case "string":
        //            break;
        //    }

        //}


        //private void SetPrimaryKey(int indent)
        //{
        //    var cols = _columns
        //        .Where(c => c.IsPrimaryKey).ToList();

        //    if (cols.Count() > 1)
        //        SetCompositeKeys(cols, 8);
        //    else if (cols.Count == 1)
        //    {
        //        var col = cols.FirstOrDefault();
        //        if (col != null) AppendText(Indent(indent) + ".HasKey(t => t." + col.ColumnName + ")");
        //    }
        //    else if (cols.Count == 0)
        //    {
        //        SetAnotherKey(8);
        //    }
        //}

        //private void SetAnotherKey(int indent)
        //{
        //    var col = _columns.FirstOrDefault(c => c.IsForeignKey);
        //    if (col != null) AppendText(Indent(indent) + "HasKey(t => t." + col.ColumnName + ");");
        //}

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
            const string left = ".HasMany(t => t.";

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