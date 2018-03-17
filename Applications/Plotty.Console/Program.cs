using Plotty.Parser;
using Plotty.VirtualMachine;
using Superpower;

namespace Plotty.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");

            var text1 = @"MOVE	R1,#5
pepito:ADD	R2,#1
	BRANCH	R1,R2,end
        BRANCH	R0,R0,pepito
end:	HALT";
            var tokenList = new Tokenizer().Tokenize(text1);
            var parsed = Parser.Parser.AsmParser.Parse(tokenList);

            var plottyCore = new PlottyMachine();
            plottyCore.Load(parsed);

            while (plottyCore.CanExecute)
            {
                plottyCore.Execute();
            }            
        }
    }
}
