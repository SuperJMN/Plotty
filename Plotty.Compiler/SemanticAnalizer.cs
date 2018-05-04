using System.Collections.Generic;
using CodeGen.Core;
using CodeGen.Parsing.Ast;

namespace Plotty.Compiler
{
    public class SemanticAnalizer
    {
        public AnalysisResult Verify(Program parsed)
        {
            //var declarations = parsed.Block.Declarations;

            //var referenceVisitor = new ReferenceCollector();

            //parsed.Accept(referenceVisitor);

            //var references = referenceVisitor.References;

            //CheckReferences(declarations, references);

            return new AnalysisResult
            {
                IsSuccess = true,
            };
        }

        private void CheckReferences(IEnumerable<DeclarationStatement> declarations, IEnumerable<Reference> references)
        {
            // TODO:
            //var declarationsSet = new HashSet<Reference>(declarations.SelectMany(s => s..Select(a => a.Reference)));
            //var referencesSet = new HashSet<Reference>(references.Where(r => !r.IsUnknown));
            //referencesSet.ExceptWith(declarationsSet);

            //if (referencesSet.Any())
            //{
            //    var variables = string.Join(",", referencesSet);
            //    throw new SemanticException($"Undeclared variable: {variables}");
            //}
        }
    }
}