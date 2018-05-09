namespace Plotty.Model
{
    public static class MachineMixin
    {
        public static void ExecuteUntilHalt(this IPlottyMachine machine)
        {
            while (machine.CanExecute)
            {
                machine.Execute();
            }
        }
    }
}