﻿namespace Plotty.Model
{
    public class StoreInstruction : Instruction
    {
        public MemoryAddress MemoryAddress { get; set; }
        public Source Source { get; set; }

        public override string ToString()
        {
            return $"Store {Source} in Memory at [{MemoryAddress}]";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}