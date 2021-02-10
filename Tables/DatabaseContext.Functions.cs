using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class DatabaseContext
    {
        private bool _preserveTableName ;
        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            AppendText();
            var table = GetTables(false);
            foreach (var item in table)
            {
                AppendText(Indent(8) + "public DbSet<" + Singularize(item.TableName,_preserveTableName) + "> " + Pluralize(item.TableName,_preserveTableName) + " { get; set; }");
            }

            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");
            foreach (var item in table)
            {
                AppendText(Indent(12) + "modelBuilder.Entity<" + Singularize(item.TableName,_preserveTableName) + ">().ToTable(" + Singularize(item.TableName,_preserveTableName).AddQuotes() + ");");
            }
            AppendText(Indent(8) + "}");
            
        }
    }
}

/*
 This is what your output should look like:
 
    public DbSet<Enrollment> Enrollments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
    }

*/
