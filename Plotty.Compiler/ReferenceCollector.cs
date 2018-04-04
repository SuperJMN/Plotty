using System.Collections.Generic;
using CodeGen.Core;
using CodeGen.Parsing.Ast;
using CodeGen.Parsing.Ast.Expressions;
using CodeGen.Parsing.Ast.Statements;

namespace Plotty.Compiler
{
    public class ReferenceCollector : ICodeVisitor
    {
        private readonly List<Reference> references = new List<Reference>();

        public void Visit(ExpressionNode expressionNode)
        {
            foreach (var op in expressionNode.Operands)
            {
                op.Accept(this);
            }

            references.Add(expressionNode.Reference);
        }

        public void Visit(ConstantExpression expression)
        {                        
        }

        public void Visit(IfStatement expression)
        {
            expression.Statement.Accept(this);
            expression.Condition.Accept(this);
            expression.ElseStatement?.Accept(this);
        }

        public void Visit(ReferenceExpression expression)
        {
            references.Add(expression.Reference);
        }

        public void Visit(AssignmentStatement expression)
        {
            expression.Assignment.Accept(this);
            references.Add(expression.Target);
        }

        public void Visit(ForLoop code)
        {
            code.Statement.Accept(this);
            code.Header.Step.Accept(this);
            code.Header.Condition.Accept(this);
            code.Header.Initialization.Accept(this);
        }

        public void Visit(WhileStatement statement)
        {
            statement.Statement.Accept(this);
            statement.Condition.Accept(this);
        }

        public void Visit(DoStatement statement)
        {
            statement.Statement.Accept(this);
            statement.Condition.Accept(this);        }

        public void Visit(AssignmentOperatorStatement statement)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(Function function)
        {
            
        }

        public void Visit(DeclarationStatement expressionNode)
        {            
        }

        public void Visit(VariableDeclaration expressionNode)
        {            
        }

        public void Visit(Program program)
        {
            foreach (var unit in program.Functions)
            {
                unit.Accept(this);
            }
        }

        public void Visit(Call call)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(ReturnStatement expressionNode)
        {
        }

        public void Visit(Argument argument)
        {           
        }

        public IEnumerable<Reference> References => references.AsReadOnly();
    }
}