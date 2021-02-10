namespace ZeraSystems.CodeNanite.Schema
{
    public partial class CurrentTable
    {
        //private void MainFunction()
        //{
        //    Output = GetTable(Input);
        //    //AppendText(GetTable(Input),"");
        //}

        private void MainFunction() => Output = Singularize(GetTable(Input,false),PreserveTableName());
    }
}