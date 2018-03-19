namespace Plotty.Model
{
    public interface ILineVisitor
    {
        void Visit(HaltInstruction instruction);
        void Visit(MoveInstruction instruction);
        void Visit(StoreInstruction instruction);
        void Visit(LoadInstruction instruction);
        void Visit(AddInstruction instruction);
        void Visit(BranchInstruction instruction);
    }
}
