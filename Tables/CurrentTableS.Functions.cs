using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeraSystems.CodeNanite.Schema
{

    public partial class CurrentTableS
    {
        void MainFunction() => Output = Pluralize(GetTable(Input, false),PreserveTableName());
    }
}
