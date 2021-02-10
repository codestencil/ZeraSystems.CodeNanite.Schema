using System.Collections.Generic;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateDbContextModelBuilder
    {
        #region Fields

        private List<ISchemaItem> _columns;
        private bool _preserveTableName ;
        #endregion Fields

        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            GetExpansionString("DB_CONTEXT");
            GetExpansionString("NAMESPACE");
            GetTable(Input, false);
            //_columns = GetColumns(_table,false);
            _columns = GetColumnsAndNavigation(Input, false);
            if (_columns == null)
                return;

            Generate();
        }

        private void Generate()
        {
            AppendText();
            var thisTable = GetTables(false);
            foreach (var item in thisTable)
            {
                AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName,_preserveTableName) + "> " + Pluralize(item.TableName,_preserveTableName) + " { get; set; }");
            }
            AppendText("");

            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");
            foreach (var item in thisTable)
            {
                var tableEntityConfig = Singularize(item.TableName,_preserveTableName) + "EntityConfiguration";
                AppendText(Indent(12) + "modelBuilder.ApplyConfiguration(new " + tableEntityConfig + "());");
                //AppendText("");
            }
            AppendText(Indent(8) + "}");
        }
    }
}