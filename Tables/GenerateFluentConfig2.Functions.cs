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
        private List<ISchemaItem> _indexRows; 
            

        private void MainFunction2()
        {
            _preserveTableName = PreserveTableName();
            _table = GetTable(Input, false);
            _tableObject = GetTableObject(Input, false);
            _columns = GetColumnsExCalculated(Input);  //GetColumns(Input, false, true);

            _indexRows = SchemaItem
                .Where(e => e.TableName == _table && e.IndexName !="" && !e.IsPrimaryKey ).ToList();

            if (_columns == null) return;
            GenerateIndex();
        }

        private void GenerateIndex()
        {
            AppendText();
            foreach (var item in _indexRows)
            {
                AppendText(FormatIndex(item, 12));
                AppendText(Indent(8) + "entity.HasIndex(e => e." + item.ColumnName + item.IndexName.AddQuotes() +")");
                AppendText(GetFluentProperties(item, 12));
            }
        }

        

        //https://msdn.microsoft.com/en-us/data/jj591617#1.2
        //https://practiceaspnet.wordpress.com/2015/10/19/configuring-one-to-many-relationships-with-fluent-api/
    }
}