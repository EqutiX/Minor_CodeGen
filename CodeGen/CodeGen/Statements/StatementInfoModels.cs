using System.CodeDom;

namespace CodeGen.Statements
{
    public interface IStatementInfo
    {
    }

    public class CommentStatementInfo : IStatementInfo
    {
        public string Text { get; set; }
    }

    public class ReturnStatementInfo : IStatementInfo
    {
        public CodeExpression Expression { get; set; }
    }

	public class AssignStatementInfo : IStatementInfo
	{
		public CodeExpression LeftExpression { get; set; }
		public CodeExpression RightExpression { get; set; }
	}

	public class AttachEventStatementInfo : IStatementInfo
	{
		public CodeEventReferenceExpression EventReferenceExpression { get; set; }
		public CodeExpression ListenerExpression { get; set; }
	}

	public class ConditionStatementInfo : IStatementInfo
	{
		public CodeExpression ConditionExpression { get; set; }
		public CodeStatement[] TrueStatements { get; set; }
		public CodeStatement[] FalseStatements { get; set; }
	}

	public class ExpressionStatementInfo : IStatementInfo
	{
		public CodeExpression Expression { get; set; }
	}

	public class GotoStatementInfo : IStatementInfo
	{
		public string Label { get; set; }
	}

	public class IterationStatementInfo : IStatementInfo
	{
		public CodeStatement InitStatement { get; set; }
		public CodeExpression TestExpression { get; set; }
		public CodeStatement IncrementStatement { get; set; }
		public CodeStatement[] Statements { get; set; }
	}

	public class LabeledStatementInfo : IStatementInfo
	{
		public string Label { get; set; }
		public CodeStatement Statement { get; set; }
	}

	public class RemoveEventStatementInfo : IStatementInfo
	{
		public CodeEventReferenceExpression EventRef { get; set; }
		public string EventName { get; set; }
		public CodeExpression ListenerExpression { get; set; }
	}

	public class SnippetStatementInfo : IStatementInfo
	{
		public string Value { get; set; }
	}

	public class ThrowExceptionStatementInfo : IStatementInfo
	{
		public CodeExpression ToThrowExpression { get; set; }
	}

	public class TryCatchFinallyStatementInfo : IStatementInfo
	{
		public CodeStatement[] TryStatements { get; set; }
		public CodeCatchClause[] CatchClauses { get; set; }
		public CodeStatement[] FinallyStatements { get; set; }
	}

	public class VariableDeclarationStatementInfo : IStatementInfo
	{
		public CodeTypeReference Type { get; set; }
		public string VariableName { get; set; }
		public CodeExpression InitExpression { get; set; }
	}
}