using System;
using System.Collections.Generic;
using System.Linq;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class Emitter
    {
        private readonly ICollection<ILine> lines;
        private readonly ICollection<PendingFixup> fixups;
        private readonly Register stackRegister;
        private readonly Register baseRegister;

        public Emitter(ICollection<ILine> lines, ICollection<PendingFixup> fixups, Register stackRegister, Register baseRegister)
        {
            this.lines = lines;
            this.fixups = fixups;
            this.stackRegister = stackRegister ?? throw new ArgumentNullException(nameof(stackRegister));
            this.baseRegister = baseRegister;
        }

        protected virtual void AddLine(Line line)
        {
            lines.Add(line);
        }

        public void Label(Label label)
        {
            AddLine(new Line(label, null));
        }

        public void Jump(Label label)
        {
            AddLine(new Line(new BranchInstruction
            {
                Operator = BooleanOperator.Equal,
                One = new Register(0),
                Another = new Register(0),
                Target = new LabelTarget(label)
            }));
        }

        public void Store(Register subject, Register @base, Register offset = null)
        {
            var source = offset == null ? (Source)new ImmediateSource(0) : new RegisterSource(offset);

            AddLine(new Line(new StoreInstruction
            {
                Source = new RegisterSource(subject),
                MemoryAddress = new IndexedAddress(@base, source)
            }));
        }

        public void Load(Register subject, Register @base, Register offset = null)
        {
            Source source;
            if (offset == null)
            {
                source = new ImmediateSource(0);
            }
            else
            {
                source = new RegisterSource(offset);
            }

            AddLine(new Line(new LoadInstruction
            {
                Destination = subject,
                MemoryAddress = new IndexedAddress(@base, source)
            }));
        }

        public void Branch(BooleanOperator op, Register r1, Register r2, Label label)
        {
            AddLine(new Line(new BranchInstruction
            {
                Operator = op,
                One = r1,
                Another = r2,
                Target = new LabelTarget(label)
            }));
        }

        public void Arithmetic(ArithmeticOperator op, Source source, Register destination, Register alternate = null)
        {
            AddLine(new Line(new ArithmeticInstruction
            {
                ArithmeticOperator = op,
                Left = destination,
                Right = source,
                Destination = alternate ?? destination
            }));
        }

        public void Increment(Register register)
        {
            AddInt(1, register);
        }

        public void Decrement(Register register)
        {
            SubstractInt(1, register);
        }

        public void Jump(Register register)
        {
            AddLine(new Line(new BranchInstruction
            {
                Operator = BooleanOperator.Equal,
                One = new Register(0),
                Another = new Register(0),
                Target = new SourceTarget(new RegisterSource(register))
            }));
        }

        public void LogVisitFor(IntermediateCode code)
        {
        }

        public void Halt()
        {
            AddLine(new Line(new HaltInstruction()));
        }

        public void Pop(Register target)
        {
            Decrement(stackRegister);
            Load(target, baseRegister, stackRegister);
        }

        public void Push(Register source)
        {
            Store(source, baseRegister, stackRegister);
            Increment(stackRegister);
        }

        public void Push(Label label)
        {
            Move(-1, 0);
            var move = GetLast();
            fixups.Add(new PendingFixup(move, new ReplaceByLabelAddressFixup(label)));

            Push(0);
        }

        private ILine GetLast()
        {
            return lines.Last();
        }

        public void AddRegister(Register source, Register destination, Register alternateDestination = null)
        {
            Arithmetic(ArithmeticOperator.Add, new RegisterSource(source), destination, alternateDestination);
        }

        public void Substract(Register source, Register destination, Register alternate = null)
        {
            Arithmetic(ArithmeticOperator.Substract, new RegisterSource(source), destination, alternate);
        }

        public void AddInt(int value, Register destination, Register alternateDestination = null)
        {
            Arithmetic(ArithmeticOperator.Add, new ImmediateSource(value), destination, alternateDestination);
        }

        public void SubstractInt(int value, Register destination, Register alternate = null)
        {
            Arithmetic(ArithmeticOperator.Substract, new ImmediateSource(value), destination, alternate);
        }

        public void Arithmetic(ArithmeticOperator op, Register register, Register second,
            Register alternate = null)
        {
            Arithmetic(op, new RegisterSource(second), register, alternate);
        }

        public void Arithmetic(ArithmeticOperator op, Register register, int value, Register alternate = null)
        {
            Arithmetic(op, new ImmediateSource(value), register, alternate);
        }

        public void Transfer(Register source, Register destination)
        {
            AddLine(new Line(new MoveInstruction
            {
                Destination = destination,
                Source = new RegisterSource(source)
            }));
        }

        public void Move(int immediate, Register destination)
        {
            AddLine(new Line(new MoveInstruction
            {
                Source = new ImmediateSource(immediate),
                Destination = destination,
            }));
        }
    }
}