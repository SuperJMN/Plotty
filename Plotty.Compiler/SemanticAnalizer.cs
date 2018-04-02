using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Parsing;
using CodeGen.Parsing.Ast;
using CodeGen.Parsing.Ast.Statements;

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
            var declarationsSet = new HashSet<Reference>(declarations.SelectMany(s => s.Declarations.Select(a => a.Reference)));
            var referencesSet = new HashSet<Reference>(references.Where(r => !r.IsUnknown));
            referencesSet.ExceptWith(declarationsSet);

            if (referencesSet.Any())
            {
                var variables = string.Join(",", referencesSet);
                throw new SemanticException($"Undeclared variable: {variables}");
            }
        }
    }
}