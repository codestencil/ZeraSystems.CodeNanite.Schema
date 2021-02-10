using System.Collections.Generic;
using System.ComponentModel.Composition;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    /// <summary>
    /// There are 6 elements in the String Array used by the 
    /// 0 - This is the name of the publisher
    /// 1 - This is the title of the Code Nanite
    /// 2 - This is the 
    /// 3 - Version Number
    /// 4 - Label of the Code Nanite
    /// 5 - Namespace
    /// 6 - Release Date
    /// 7 - Name to use for Expander Label
    /// 9 - RESERVED
    /// 10 - RESERVED
    /// </summary>
    [Export(typeof(ICodeStencilCodeNanite))]
    [CodeStencilCodeNanite(new[]
    {
        "Zera Systems Inc.",
        "Current Table Getter in lowercase (Pluralized)",
        "Get The Current Table Pluralized (in lowercase)",
        "1.0",
        "CurrentTableSLowerCase",
        "ZeraSystems.CodeNanite.Schema",
        "10/01/2019",
        "CS_CURRENT_TABLES_LOWER",
        "1",
        "",
        "https://codestencil.com/zerasystems.schema/current-tables-lowercase"
    })]
    public partial class CurrentTableSLowerCase : ExpansionBase, ICodeStencilCodeNanite
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Counter { get; set; }
        public List<string> OutputList { get; set; }
        public List<ISchemaItem> SchemaItem { get; set; }
        public List<IExpander> Expander { get; set; }
        public List<string> InputList { get; set; }

        public void ExecutePlugin()
        {
            Initializer(SchemaItem, Expander);
            MainFunction();
            //Output = ExpandedText.ToString();
        }
    }
}

