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
        "Pluralized Current Table Getter",
        "Get the pluralized name for the Current Table. This is dependent on CS_TABLE_LIST when specified as part of a node label. Code will be generated for tables in the schema.",
        "1.0",                                  
        "CurrentTables",
        "ZeraSystems.CodeNanite.Schema",
        "07/07/2017",
        "CS_CURRENT_TABLES",                        
        "1",                                    
        "",
        "https://codestencil.com/zerasystems.schema/current-tables"
    })]
    public partial class CurrentTableS : ExpansionBase, ICodeStencilCodeNanite
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

