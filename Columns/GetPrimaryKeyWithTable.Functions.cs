using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class GetPrimaryKeyWithTable
    {
        private void MainFunction()
        {
            Output = GetTable(Input, false) +"."+ GetPrimaryKey(Input) ;
        }
    }
}