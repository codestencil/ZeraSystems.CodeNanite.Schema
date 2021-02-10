namespace ZeraSystems.CodeNanite.Schema
{
    public partial class CurrentTableLowerCase
    {
        private void MainFunction() => Output = GetTable(Input, false).ToLower();
    }
}