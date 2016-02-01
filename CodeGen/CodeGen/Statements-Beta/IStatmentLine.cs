using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace CodeGen
{
   public interface IStatementLine
    {
        Dictionary<int,IExpressionLine> Expressions { get; set; }

        CodeStatement CreateStatement();

    }
}


namespace CodeGen.Statements
{
    public class CommentStatementLine : IStatementLine
    {
        public bool? IsDoc { get; set; }

        public string Text { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return IsDoc == null
                ? new CodeCommentStatement(Text)
                : new CodeCommentStatement(Text,
                    IsDoc.Value);
        }
    }

    public class ReturnStatementLine : IStatementLine
    {
        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeMethodReturnStatement(Expressions[0].CreateExpression());
        }
    }

    public class AssignStatementLine : IStatementLine
    {
        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeAssignStatement(Expressions[0].CreateExpression(),Expressions[1].CreateExpression());
        }
    }

    public class AttachEventStatementLine:IStatementLine
    {
        public string EventName { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return
                new CodeAttachEventStatement(
                    new CodeEventReferenceExpression(Expressions[0].CreateExpression(), EventName),
                    Expressions[1].CreateExpression());
        }
    }

    public class ConditionStatementLine:IStatementLine
    {
        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public IStatementLine[] TrueStatementLines { get; set; }

        public IStatementLine[] FalseStatementLines { get; set; }

        public CodeStatement CreateStatement()
        {
            var trueStatements = TrueStatementLines.Select(e => e.CreateStatement()).ToArray();
            var falseStatements = FalseStatementLines.Select(e => e.CreateStatement()).ToArray();
           return new CodeConditionStatement(Expressions[0].CreateExpression(), trueStatements, falseStatements);
        }
    }

    public class ExpressionStatementLine: IStatementLine
    {
        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeExpressionStatement(Expressions[0].CreateExpression()); 
        }
    }

    public class GotoStatementLine: IStatementLine
    {
        public string Label { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeGotoStatement(Label);
        }
    }

    public class LabledStatementLine: IStatementLine
    {
        public string Label { get; set; }

        public IStatementLine StatementLine { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return StatementLine == null
                ? new CodeLabeledStatement(Label)
                : new CodeLabeledStatement(Label, StatementLine.CreateStatement());
        }
    }

    public class IterationStatementLine : IStatementLine
    {
        public IStatementLine Init { get; set; }

        public IStatementLine Increment { get; set; }

        public IStatementLine[] StatementLines { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }
        public CodeStatement CreateStatement()
        {
            return new CodeIterationStatement(Init.CreateStatement(), Expressions[0].CreateExpression(),
                Increment.CreateStatement(), StatementLines.Select(s => s.CreateStatement()).ToArray());
        }
    }

    public class SnippetStatementLine : IStatementLine
    {
        public string Value { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeSnippetStatement(Value);
        }
    }

    public class RemoveEventStatementLine: IStatementLine
    {
        public string  EventName { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return new CodeRemoveEventStatement(new CodeEventReferenceExpression(Expressions[0].CreateExpression(), EventName),
                    Expressions[1].CreateExpression());
        }
    }

    public class ThrowExceptionStatementLine: IStatementLine
    {
        public Dictionary<int, IExpressionLine> Expressions { get; set; }
        public CodeStatement CreateStatement()
        {
            return new CodeThrowExceptionStatement(Expressions[0].CreateExpression());
        }
    }

    public class TryCatchFinallyStatementLine: IStatementLine
    {

        public IStatementLine[] Try { get; set; }


        public string LocalName { get; set; }

        public Type Type { get; set; }

        public IStatementLine[] Catch { get; set; }

        public IStatementLine[] Finally { get; set; }
        public Dictionary<int, IExpressionLine> Expressions { get; set; }
        public CodeStatement CreateStatement()
        {
            CodeCatchClause codeCatchClause;

            if (Type != null && Catch != null)
                codeCatchClause = new CodeCatchClause(LocalName, new CodeTypeReference(Type),
                    Catch.Select(s => s.CreateStatement()).ToArray());
            else if (Type != null && Catch == null)
                codeCatchClause = new CodeCatchClause(LocalName,new CodeTypeReference(Type));
            else
                codeCatchClause = new CodeCatchClause(LocalName);


            return Finally != null
                ? new CodeTryCatchFinallyStatement(Try.Select(s => s.CreateStatement()).ToArray(), new[] {codeCatchClause},
                    Finally.Select(s => s.CreateStatement()).ToArray())
                : new CodeTryCatchFinallyStatement(Try.Select(s => s.CreateStatement()).ToArray(),
                    new[] {codeCatchClause});
        }
    }

    public class VariableDeclarationStatementLine: IStatementLine
    {
        public Type Type { get; set; }

        public string Name { get; set; }

        public Dictionary<int, IExpressionLine> Expressions { get; set; }

        public CodeStatement CreateStatement()
        {
            return Expressions == null
                ? new CodeVariableDeclarationStatement(Type, Name)
                : new CodeVariableDeclarationStatement(Type, Name, Expressions[0].CreateExpression());

        }
    }
}
