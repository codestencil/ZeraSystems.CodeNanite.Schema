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
        "On Creating ModelBuilder",
        "Generate On Creating ModelBuilder",
        "1.0",
        "GenerateOnModelCreating",
        "ZeraSystems.CodeNanite.Schema",
        "02/16/2020",
        "CS_GENERATE_ON_MODEL_CREATING",
        "1",
        "",
        "https://codestencil.com/zerasystems.schema/generatefluentconfig"
    })]
    public partial class GenerateOnModelCreating : ExpansionBase, ICodeStencilCodeNanite
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Counter { get; set; }
        public List<string> OutputList { get; set; }
        public List<ISchemaItem> SchemaItem { get; set; }
        public List<IExpander> Expander { get; set; }
        public List<string> InputList { get; set; }

        #region Fields

        private string _table;
        private List<ISchemaItem> _columns;
        private ISchemaItem _tableObject;
        private bool _preserveTableName ;
        private bool _total12M = false;
        const int indent = 8;

        public enum Reverse
        {
            WithRequired,
            WithOptional
        }

        #endregion Fields

        public void ExecutePlugin()
        {
            //if (Input.IsBlank()) return;

            Initializer(SchemaItem, Expander);
            MainFunction();
            Output = ExpandedText.ToString();
        }
    }
}