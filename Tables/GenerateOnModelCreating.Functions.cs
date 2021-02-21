using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GenerateOnModelCreating
    {
        private List<ISchemaItem> _indexRows; 
            
        private void MainFunction()
        {
            _preserveTableName = PreserveTableName();
            _indexRows = SchemaItem
                .Where(e => e.IsForeignKey)
                .Where(e => e.IndexName != null)
                .Where(e => !e.IsPrimaryKey ).ToList();

            AppendText();
            var thisTable = GetTables(false);
            AppendText(Indent(8) + "protected override void OnModelCreating(ModelBuilder modelBuilder)");
            AppendText(Indent(8) + "{");

            foreach (var row in thisTable)
            {
                AppendText(ModelBuilder(row));
            }
            AppendText(Indent(8) + "}");
        }

        private string ModelBuilder(ISchemaItem row)
        {
            var table = row.TableName;
            BuildSnippet("modelBuilder.Entity<"+table+">(entity =>", 12);
            BuildSnippet("{;", 12);

            HasIndex(table);
            HasOneWithMany(table);
            //BuildSnippet(FormatOneToMany(row, 12));
            BuildSnippet("});", 12);
            return BuildSnippet();
        }

        private void HasIndex(string table)
        {
            var indexColumns = SchemaItem
                .Where(x => x.TableName == table && x.IndexName != null)
                .Where(p => p.IsPrimaryKey == false)
                .ToList();
            foreach (var column in indexColumns)
            {
                BuildSnippet("entity.HasIndex(e => e." + column.ColumnName + ")" +
                             ".HasName(" + column.IndexName.AddQuotes() + ");", 16);
            }
        }

        private void HasOneWithMany(string table)
        {
            var oneToMany = SchemaItem
                .Where(e => e.TableName == table)
                .Where(e => e.IsForeignKey)
                .Where(e => e.RelatedTable != null)
                .Where(e => !e.IsPrimaryKey)
                .Select(x => new {x.RelatedTable, x.TableName, x.ColumnName, x.ConstraintName})
                .ToList();
            foreach (var column in oneToMany)
            {
                BuildSnippet("entity.HasOne(d => d." + column.RelatedTable + ")".AddCarriage() + 
                             Space(20)+".WithMany(p => p." + column.TableName + ")".AddCarriage() +
                             Space(20)+".HasForeignKey(d => d." + column.ColumnName + ")".AddCarriage() +
                             Space(20)+".HasConstraintName(" + column.ConstraintName + ")".AddCarriage(), 16);
            }
            
        }

        public static string Space(int length)
        {
            return "".PadRight(length);
        }
        /*
                entity.HasOne(d => d.Engineer)
                    .WithMany(p => p.ProductsEngineer)
                    .HasForeignKey(d => d.EngineerId)
                    .HasConstraintName("FK_dbo.Products_dbo.Employees_EngineerId");
         
         */

        private string FormatOneToMany(ISchemaItem row, int tab)
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

                if (row.RelatedTable != row.TableName)  //Indicating a table related to itself
                    result += Indent(tab - 4) + "entity.HasOne(d => d." + CreateTablePropertyName(row) + ")".AddCarriage();
                else
                    result += Indent(tab - 4) + "entity.HasOne(d => d." + row.SelfReferenceColumn() + ")".AddCarriage();

                if (row.RelatedTable == row.TableName)  //Indicating a table related to itself
                    result += Indent(tab) + ".WithMany(p => p." + row.SelfRefNavProperty() + ")".AddCarriage();
                else
                    result += Indent(tab) + ".WithMany(p => p." + Pluralize(_table,_preserveTableName) + ")".AddCarriage() ;

                result += Indent(tab) + ".HasForeignKey(d => d." + row.ColumnName + ")";

                if (row.ConstraintName.IsBlank())
                    result += ";";
                else
                    result += "".AddCarriage() +Indent(tab) + ".HasConstraintName(" + row.ConstraintName.AddQuotes()+");";
            }

            return result;
        }


        

        //https://msdn.microsoft.com/en-us/data/jj591617#1.2
        //https://practiceaspnet.wordpress.com/2015/10/19/configuring-one-to-many-relationships-with-fluent-api/
    }
}