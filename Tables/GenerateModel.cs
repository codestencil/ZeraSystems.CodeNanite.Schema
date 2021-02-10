using System.Collections.Generic;
using System.ComponentModel.Composition;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    /// <summary>
    /// There are 10 elements in thHhe String Array used by the
    /// 0  - This is the name of the publisher
    /// 1  - This is the title of the Code Nanite
    /// 2  - This is the description
    /// 3  - Version Number
    /// 4  - Label of the Code Nanite
    /// 5  - Namespace
    /// 6  - Release Date
    /// 7  - Name to use for Expander Label
    /// 9  - RESERVED
    /// 10 - RESERVED
    /// </summary>
    [Export(typeof(ICodeStencilCodeNanite))]
    [CodeStencilCodeNanite(new[]
    {
        "Zera Systems Inc.",
        "Model Generator",
        "Generates Models from Schema",
        "1.1",
        "GenerateModel",
        "ZeraSystems.CodeNanite.Schema",
        "5/2/2020",
        "CS_GENERATE_MODEL",
        "1",
        "",
        ""
    })]
    public partial class GenerateModel : ExpansionBase, ICodeStencilCodeNanite
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Counter { get; set; }
        public List<string> OutputList { get; set; }
        public List<ISchemaItem> SchemaItem { get; set; }
        public List<IExpander> Expander { get; set; }
        public List<string> InputList { get; set; }

        //public string ErrorMessage { get; set; }

        public void ExecutePlugin()
        {
            OutputList = new List<string>();
            if (Input == null)
            {
                //OutputList.Add("**ERROR : Input = null, Missing Table Name");
                return;
            }

            Initializer(SchemaItem, Expander);
            MainFunction();
            Output = ExpandedText.ToString();
        }
    }
}