namespace Plotty.Model
{
    public interface ILineVisitor
    {
        void Visit(HaltInstruction instruction);
        void Visit(MoveInstruction instruction);
        void Visit(StoreInstruction instruction);
        void Visit(LoadInstruction instruction);
        void Visit(BranchInstruction instruction);
        void Visit(Line line);
        void Visit(ArithmeticInstruction instruction);
        void Visit(NoOperation instruction);
    }
}
