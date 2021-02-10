using System.Collections.Generic;
using System.ComponentModel.Composition;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    /// <summary>
    /// There are 6 elements in the String Array used by the
    /// 0 - Publisher : This is the name of the publisher
    /// 1 - Title : This is the title of the Code Nanite
    /// 2 - Details : This is the
    /// 3 - Version Number
    /// 4 - Label : Label of the Code Nanite
    /// 5 - Namespace
    /// 6 - Release Date
    /// 7 - Name to use for Expander Label
    /// 8 - Indicates that the Nanite is Schema Dependent
    /// 9 - RESERVED
    /// 10 - RESERVED
    /// </summary>
    [Export(typeof(ICodeStencilCodeNanite))]
    [CodeStencilCodeNanite(new[]
    {
        "Zera Systems Inc.",                        // 0 - Publisher : This is the name of the publisher
        "Get the list of Column Captions",          // 1 - Title : This is the title of the Code Nanite
        "Returns a list of Column Types of the "+
        "passed table.",                            // 2
        "1.0",                                      // 3
        "GetColumnCaptions",                        // 4
        "ZeraSystems.CodeNanite.Schema",            // 5
        "03/29/2020",                               // 6
        "CS_COLUMN_CAPTION",                        // 7
        "1",                                        // 8
        "",                                         // 9
        "https://codestencil.com/documentation/how-codestencil-works"                                          // 10
    })]
    public partial class GetColumnCaptions : ExpansionBase, ICodeStencilCodeNanite
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