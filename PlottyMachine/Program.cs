using System;
using System.Collections.Generic;
using Plotty;

namespace PlottyMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var plottyCore = new PlottyCore();
            plottyCore.Load(new List<Instruction>()
            {
                new Instruction()
                {
                    OpCode = OpCodes.Load,
                    Address = 0,
                    Registers = new List<Register>() { new Register(0)},
                }
            });

            plottyCore.Execute();
        }
    }
}
