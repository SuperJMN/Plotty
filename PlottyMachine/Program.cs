using System;
using Plotty;
using Superpower;

namespace PlottyMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var text1 = @"LOAD	R1,#5
pepito:ADD	R2,#1
	BRANCH	R1,R2,end
        BRANCH	R0,R0,pepito
end:	HALT";
            var tokenList = new Tokenizer().Tokenize(text1);
            var parsed = Parser.AsmParser.Parse(tokenList);

            var plottyCore = new PlottyCore();
            plottyCore.Load(parsed);

            while (plottyCore.CanExecute)
            {
                plottyCore.Execute();
            }            
        }
    }
}
