using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateFluentConfig
    {
        private List<ISchemaItem> _indexRows; 
            

        private void MainFunction2()
        {
            _preserveTableName = PreserveTableName();
            _table = GetTable(Input, false);
            _tableObject = GetTableObject(Input, false);
            _columns = GetColumnsExCalculated(Input);  //GetColumns(Input, false, true);

            _indexRows = SchemaItem
                .Where(e => e.TableName == _table && e.IndexName !=null && !e.IsPrimaryKey ).ToList();

            if (_columns == null) return;
            GenerateIndex();
            GenerateSpecificColumns();
            GenerateFKEntities();
        }

        private void GenerateIndex()
        {
            AppendText();
            foreach (var item in _indexRows)
            {
                //AppendText(FormatIndex(item, 12));
                AppendText(Indent(12) + "entity.HasIndex(e => e." + item.ColumnName +", " + item.IndexName.AddQuotes() +");");
                //AppendText(GetFluentProperties(item, 12));
            }
        }

        private void GenerateSpecificColumns()
        {
            //AppendText();
            var columns = _columns
                .Where(x => (x.ColumnType == "DateTime" || x.ColumnType == "Decimal"));
            foreach (var column in columns)
            {
                if (column.ColumnType == "DateTime")
                {
                    AppendText(Indent(12) + 
                               "entity.Property(e => e."+column.ColumnName+").HasColumnType("+"datetime".AddQuotes()+");");
                }
                if (column.ColumnType == "decimal")
                {
                    AppendText(Indent(12) + 
                               "entity.Property(e => e."+column.ColumnName+").HasColumnType("+("decimal("+column.NumericPrecision+", "+column.Scale).AddQuotes()+");");
                }
            }
        }

        private void GenerateFKEntities()
        {
            //AppendText();
            var columns = _columns
                .Where(x => x.IsForeignKey);
            
            foreach (var column in columns)
            {
                AppendText(FormatOneToMany(column, indent+8));

            }
        }

        

        //https://msdn.microsoft.com/en-us/data/jj591617#1.2
        //https://practiceaspnet.wordpress.com/2015/10/19/configuring-one-to-many-relationships-with-fluent-api/
    }
}