using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using CodeGen.Intermediate.Codes.Common;
using CodeGen.Parsing.Ast;
using Plotty.Model;
using ArithmeticOperator = Plotty.Model.ArithmeticOperator;
using Label = Plotty.Model.Label;

namespace Plotty.CodeGeneration
{
    public class PlottyCodeGenerationVisitor : IIntermediateCodeVisitor
    {
        public const int ReturnRegisterIndex = 5;

        private readonly Register returnRegister = new Register(ReturnRegisterIndex);
        private readonly Register baseRegister = new Register(6);
        private readonly Register stackRegister = new Register(7);

        private readonly List<PendingFixup> fixups = new List<PendingFixup>();
        private readonly List<ILine> lines = new List<ILine>();
        private SymbolTable currentSymbolTable;

        public PlottyCodeGenerationVisitor(SymbolTable symbolTable, Func<ICollection<ILine>, ICollection<PendingFixup>, Register, Register, Emitter> emitterFactory)
        {
            CurrentSymbolTable = symbolTable;
            RootSymbolTable = CurrentSymbolTable;
            Emit = emitterFactory(lines, fixups, stackRegister, baseRegister);
        }

        private Emitter Emit { get; }

        public IEnumerable<ILine> Lines => lines.AsReadOnly();

        private SymbolTable CurrentSymbolTable
        {
            get => currentSymbolTable;
            set
            {
                currentSymbolTable = value;
                Allocate(currentSymbolTable.Symbols);
            }
        }

        public SymbolTable RootSymbolTable { get; }

        public ReadOnlyCollection<PendingFixup> Fixups => fixups.AsReadOnly();

        public void Visit(JumpIfFalse code)
        {
            var zero = new Register(0);
            var register = new Register(1);

            LoadReference(code.Reference, register);

            var onFalseLabel = new Label();

            // Comparison with 0 (TRUE). If both values are 0, the condition is true, so the code has to be executed.
            // So we emit
            Emit.Move(0, zero);
            Emit.Branch(BooleanOperator.Equal, register, zero, onFalseLabel);
            // TODO: VERIFY: Emit.Move(0, zero);
            Emit.Jump(new Label(code.Label.Name));
            Emit.Label(onFalseLabel);
        }

        public void Visit(BoolConstantAssignment code)
        {
            Emit.Move(code.Value ? 0 : 1, new Register(1));
            StoreReference(code.Target, 1);
        }

        public void Visit(LabelCode code)
        {
            Emit.Label(new Label(code.Label.Name));
        }

        public void Visit(IntegerConstantAssignment code)
        {
            var tmp = new Register(1);

            Emit.Move(code.Value, tmp);
            StoreReference(code.Target, tmp);
        }

        public void Visit(ArithmeticAssignment code)
        {
            var left = new Register(1);
            var right = new Register(2);

            LoadReference(code.Left, left);
            LoadReference(code.Right, right);

            Emit.Arithmetic(GetOperator(code.Operation), new RegisterSource(right), left);
            
            StoreReference(code.Target, left);
        }

        private void LoadReference(Reference reference, Register register, Register addressRegister = null)
        {
            addressRegister = addressRegister ?? new Register(0);
            Emit.Move(GetAddress(reference), addressRegister);
            Emit.Load(register, baseRegister, addressRegister);
        }

        private void StoreReference(Reference reference, Register register, Register addressRegister = null)
        {
            addressRegister = addressRegister ?? new Register(0);
            Emit.Move(GetAddress(reference), addressRegister);
            Emit.Store(register, baseRegister, addressRegister);
        }

        public void Visit(ReferenceAssignment code)
        {
            var tmp = new Register(1);
            LoadReference(code.Origin, tmp);
            StoreReference(code.Target, tmp);
        }

        public void Visit(BoolExpressionAssignment code)
        {
            var left = new Register(1);
            var right = new Register(2);

            LoadReference(code.Left, left);
            LoadReference(code.Right, right);

            if (code.Operation == BooleanOperation.IsEqual)
            {               
                Emit.Arithmetic(ArithmeticOperator.Substract, new RegisterSource(right), left);

                StoreReference(code.Target, left);
            }
            else
            {
                var jumpOnTrue = new Label();
                var endLabel = new Label();

                Emit.Branch(code.Operation.ToOperator(), left, right, jumpOnTrue);

                // Sets false
                Emit.Move(1, left);
                Emit.Jump(endLabel);
                Emit.Label(jumpOnTrue);

                // Sets true
                Emit.Move(0, left);

                // End
                Emit.Label(endLabel);

                StoreReference(code.Target, left);
            }
        }

        public void Visit(Jump code)
        {
            Emit.Jump(new Label(code.Label.Name));
        }

        public void Visit(FunctionDefinitionCode code)
        {
            CurrentSymbolTable = GetFunctionScope(code.Firm.Name);

            Emit.Label(new Label(code.Firm.Name));

            PopParamsToFunctionSpace(code.Firm.Arguments);
        }

        public void Visit(CallCode code)
        {
            var continuationLabel = new Label();

            CreateFrame(code.FunctionName, continuationLabel);

            Emit.Jump(new Label(code.FunctionName));
            Emit.Label(continuationLabel);

            RestorePreviousFrame(code.FunctionName);
            
            if (code.Reference != null)
            {
                StoreReference(code.Reference, returnRegister);
            }
        }

        private void PopParamsToFunctionSpace(ICollection<Argument> arguments)
        {
            if (!arguments.Any())
            {
                return;
            }

            var @base = new Register(0);
            var offset = new Register(1);
            var parameter = new Register(2);
            var localAddress = new Register(3);

            Emit.Move(0, offset);
            Emit.Transfer(baseRegister, @base);
            Emit.SubstractInt(arguments.Count, @base);

            foreach (var argument in arguments)
            {
                Emit.Load(parameter, @base, offset);
                StoreReference(argument.Item.Reference, parameter, localAddress);
                Emit.Increment(offset);
            }
        }

        public void Visit(ReturnCode code)
        {
            if (code.Reference != null)
            {
                LoadReference(code.Reference, returnRegister);                
            }

            Emit.Pop(0);
            Emit.Jump(0);
        }

        public void Visit(HaltCode code)
        {
            Emit.Halt();
        }

        public void Visit(ParameterCode code)
        {
            var destination = new Register(1);
            
            LoadReference(code.Reference, destination);
            Emit.Push(destination);
        }

        public void Visit(AddressOf code)
        {
            var addressOfReg = new Register(1);
            
            Emit.Move(GetAddress(code.Source), addressOfReg);
            StoreReference(code.Target, addressOfReg);
        }

        public void Visit(ContentOf code)
        {
            var addressReg = new Register(1);
            var contentReg = new Register(2);

            LoadReference(code.Source, addressReg);
            Emit.Load(contentReg, baseRegister, addressReg);

            StoreReference(code.Target, contentReg);
        }

        public void Visit(LoadFromArray code)
        {
            // a = b[index]

            var index = new Register(1);
            LoadReference(code.Index, index);

            var bReg = new Register(2);
            Emit.Move(GetAddress(code.Source), bReg);

            Emit.AddRegister(index, bReg);

            var data = new Register(3);
            Emit.Load(data, baseRegister, bReg);
            
            StoreReference(code.Target, data);
        }

        public void Visit(StoreToArray code)
        {
            var localAddr = new Register(1);
            var offset = new Register(2);
            var data = new Register(3);

            LoadReference(code.Source, data);
            
            Emit.Move(GetAddress(code.Target), localAddr);
            LoadReference(code.Index, offset);
            Emit.AddRegister(offset, localAddr);

            Emit.Store(data, baseRegister, localAddr);            
        }
        
        private void CreateFrame(string functionName, Label label)
        {
            var baseBackup = new Register(0);
            var stackBackup = new Register(1);

            Emit.Transfer(baseRegister, baseBackup);
            Emit.Transfer(stackRegister, stackBackup);

            // New base register
            Emit.AddRegister(stackRegister, baseRegister);

            // New stack register
            var functionScope = GetFunctionScope(functionName);

            Emit.Move(functionScope.Size, stackRegister);

            Emit.Push(baseBackup);   // Base Register
            Emit.Push(stackBackup);   // Stack Register
            Emit.Push(label);
        }

        private void RestorePreviousFrame(string codeFunctionName)
        {
            var baseBackup = new Register(1);
            var stackBackup = new Register(0);

            Emit.Pop(stackBackup);    // Stack Register
            Emit.Pop(baseBackup);    // Base Register

            Emit.Transfer(stackBackup, stackRegister);
            Emit.Transfer(baseBackup, baseRegister);

            var argumentsCount = GetFunction(codeFunctionName).Arguments.Count;
            if (argumentsCount > 0)
            {
                Emit.SubstractInt(argumentsCount, stackRegister);
            }
        }

        private Function GetFunction(string functionName)
        {
            return RootSymbolTable.Children.Select(x => x.Owner).OfType<Function>().Single(x => x.Name == functionName);
        }

        private SymbolTable GetFunctionScope(string functionName)
        {
            return RootSymbolTable.Children.Single(s => (s.Owner as Function)?.Name == functionName);
        }

        private void Allocate(IReadOnlyDictionary<Reference, Properties> currentScopeSymbols)
        {
            var offset = 0;
            foreach (var pair in currentScopeSymbols)
            {
                pair.Value.Offset = offset;
                offset += pair.Value.Size;
            }
        }

        private int GetAddress(Reference reference)
        {
            return CurrentSymbolTable.Symbols[reference].Offset;
        }

        private ArithmeticOperator GetOperator(CodeGen.Intermediate.Codes.Common.ArithmeticOperator codeOperation)
        {
            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Add)
            {
                return ArithmeticOperator.Add;
            }

            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Substract)
            {
                return ArithmeticOperator.Substract;
            }

            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Mult)
            {
                return ArithmeticOperator.Multiply;
            }

            throw new ArgumentOutOfRangeException(nameof(codeOperation));
        }
    }
}