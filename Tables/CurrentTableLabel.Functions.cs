namespace ZeraSystems.CodeNanite.Schema
{
    public partial class CurrentTableLabel
    {
        private void MainFunction() => Output = Singularize(GetTableLabel(Input,false),PreserveTableName());
    }
}