using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.CodeNanite.Schema
{
    /// <summary>
    /// Returns the current table pluralized and in lowercase
    /// Implements the <see cref="ZeraSystems.CodeNanite.Expansion.ExpansionBase" />
    /// Implements the <see cref="ZeraSystems.CodeStencil.Contracts.ICodeStencilCodeNanite" />
    /// </summary>
    /// <seealso cref="ZeraSystems.CodeNanite.Expansion.ExpansionBase" />
    /// <seealso cref="ZeraSystems.CodeStencil.Contracts.ICodeStencilCodeNanite" />
    public partial class CurrentTableSLowerCase
    {
        

        private void MainFunction()
        {
            var table = GetTable(Input);
            //if (table.IsBlank())
            //{
            //    table = Singularize(Input);
            //    table = char.ToUpper(table[0]) + table.Substring(1);
            //}

            
            Output = Pluralize(GetTable(table, false).ToLower(), PreserveTableName());
        }
    }
}