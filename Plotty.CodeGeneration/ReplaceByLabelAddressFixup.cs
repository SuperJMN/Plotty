using System.Collections.Generic;
using System.Linq;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    internal class ReplaceByLabelAddressFixup : LineFixer
    {
        public Label Label { get; }

        public ReplaceByLabelAddressFixup(Label label)
        {
            Label = label;
        }

        public override void Fix(ILine toFix, IList<ILine> compilationResult)
        {
            var instWithLabel = compilationResult.First(l => l.Label == Label);
            var index = compilationResult.IndexOf(instWithLabel);

            if (toFix.Instruction is MoveInstruction move)
            {
                move.Source = new ImmediateSource(index);
            }
        }
    }
}