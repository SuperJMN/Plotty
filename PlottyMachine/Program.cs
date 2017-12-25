using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Plotty;
using Superpower;

namespace PlottyMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var text = "LOAD R0,#1\nLOAD R1,#2\nSTORE R0,0\nSTORE R1,1";
            var parsed = Parser.AsmParser.Parse(new Tokenizer().Tokenize(text));

            var plottyCore = new PlottyCore();
            plottyCore.Load(parsed);
            //{
            //    new Instruction()
            //    {
            //        OpCode = OpCodes.Load,
            //        Address = new LoadParam() {Value = 134, IsDirect = true},
            //        Registers = new List<Register>() {new Register(0)},
            //    },
            //    new Instruction()
            //    {
            //        OpCode = OpCodes.Store,
            //        Address = new LoadParam() {Value = 0,},
            //        Registers = new List<Register>() {new Register(0)},
            //    }
            //});

            plottyCore.Execute();
            plottyCore.Execute();
            plottyCore.Execute();
            plottyCore.Execute();
        }
    }
}
