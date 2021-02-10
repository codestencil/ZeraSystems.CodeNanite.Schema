using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class ExpandModels
    {
        private readonly string _public = "public ";
        private readonly string _getSet = " { get; set; }";
        private readonly string _virtual = "virtual ";
        private bool _preserveTableName ;
        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();

            AppendText();
            BuildSnippet(null);
            BuildSnippet("");
            BuildSnippet(_public+"class " + Singularize(Input, _preserveTableName),4) ;
            BuildSnippet("{",4);
            AppendText(GetColumns());
            BuildSnippet("}", 4);
            AppendText(BuildSnippet());
            //AppendText(Indent(0) + "}");
        }

        private readonly StringBuilder _iCollectionList = new StringBuilder();

        private string GetColumns(bool ignoreCalculated = true)
        {
            _iCollectionList.Clear();
            var columns = GetColumnsAndNavigation(Input);

            foreach (var item in columns)
            {
                if (ignoreCalculated && item.IsCalculatedColumn) continue;

                if (item.IsForeignKey && item.TableName != Input)
                    GetCollectionNavigation(item);
                else
                    BuildSnippet(GetColumnProperty(item));

                if (item.IsForeignKey && item.TableName == Input)
                {
                    if (item.RelatedTable == Input) //A table related to itself
                    {
                        BuildSnippet(_public + _virtual + item.RelatedTable + " " + item.ColumnName + NavigationLabel() + _getSet);
                        GetCollectionNavigation(item.TableName, item.ColumnName + NavigationLabel());
                    }
                    else
                        BuildSnippet(_public + _virtual + item.RelatedTable + " " + item.RelatedTable + _getSet);
                }
            }
            BuildSnippet(_iCollectionList.ToString());
            return BuildSnippet();
        }


        private string GetColumnProperty(ISchemaItem item )
        {
            BuildSnippet(_public + item.ColumnType + GetNullSign(item) + " " + item.ColumnName + _getSet,8,true);
            return BuildSnippet();
        }

        public new static string GetNullSign(ISchemaItem item) => item.AllowDbNull ? "?" : string.Empty;

        private void GetCollectionNavigation(ISchemaItem item)
        {
            var column = Singularize(item.TableName,_preserveTableName);
            var columns = Pluralize(item.TableName,_preserveTableName);
            BuildSnippet(_iCollectionList, _public +"virtual ICollection<" + column + "> " + columns + _getSet+" = new HashSet<" + column + ">();",8);
        }
        private void GetCollectionNavigation(string item, string inverse)
        {
            BuildSnippet(_iCollectionList, _public + "virtual ICollection<" + item + "> " + inverse + _getSet + " = new HashSet<" + item + ">();");
        }

        private void GetHashSet(List<ISchemaItem> columns, string theIndent)
        {
            AppendText(theIndent + "public " + Singularize(Input,_preserveTableName) + "()");
            AppendText(theIndent + "{");
            foreach (var item in columns)
            {
                if (item.IsForeignKey && item.TableName!= Input)
                    AppendText(theIndent + Indent(4) + "this." + Pluralize(item.TableName,_preserveTableName) + " = new HashSet<" + Singularize(item.TableName,_preserveTableName) + ">();");
            }
            AppendText(theIndent + "}");
        }
    }
}