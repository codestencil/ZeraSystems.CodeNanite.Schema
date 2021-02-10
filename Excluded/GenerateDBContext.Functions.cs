using System.Collections.Generic;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateDbContext
    {
        #region Fields

        private string _namespace;
        private string _table;
        private List<ISchemaItem> _columns;
        private string _dbcontext;
        private bool _preserveTableName ;
        #endregion Fields

        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _dbcontext = GetExpansionString("DB_CONTEXT");
            _namespace = GetExpansionString("NAMESPACE");
            _table = GetTable(Input, false);
            //_columns = GetColumns(_table,false);
            _columns = GetColumnsAndNavigation(Input, false);
            if (_columns == null)
                return;

            Generate();
        }

        private void Generate()
        {
            AppendText();
            var thistable = GetTables(false);
            foreach (var item in thistable)
            {
                AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName,_preserveTableName) + "> " + Pluralize(item.TableName,_preserveTableName) + " { get; set; }");
            }
            AppendText("");

            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");
            var noLineFeed = string.Empty;
            foreach (var item in thistable)
            {
                var tableEntityConfig = Singularize(item.TableName,_preserveTableName) + "EntityConfiguration";
                AppendText(Indent(12) + "modelBuilder.ApplyConfiguration(new " + tableEntityConfig + "());", noLineFeed) ;
            }
            AppendText(Indent(8) + "}");
        }

    }
}