using System;
using System.CodeDom;

namespace CodeGen.Statements
{
	public abstract class BaseStatementHelper
	{
		public abstract bool Supports(CodeStatementType codeStatementType);

		// TODO: some implementations of this function have more then 1 usefull overload.
		public abstract CodeStatement CreateStatement(IStatementInfo statementInfo);

		//ToDO: return simple model  that can be generated in a non CodeDom Knowning environment. that later can be used to Create the statment in a CodeDom environment
	}

	public class CommentStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.CommentStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as CommentStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeCommentStatement(info.Text);
		}
	}

	public class ReturnStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.ReturnStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as ReturnStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeMethodReturnStatement(info.Expression);
		}
	}

	public class AssignStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.AssignStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as AssignStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeAssignStatement(info.LeftExpression, info.RightExpression);
		}
	}

	public class AttachEventStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.AttachEventStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as AttachEventStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeAttachEventStatement(info.EventReferenceExpression, info.ListenerExpression);
		}
	}

	public class ConditionStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.ConditionStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as ConditionStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeConditionStatement(info.ConditionExpression, info.TrueStatements, info.FalseStatements);
		}
	}

	public class ExpressionStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.ExpressionStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as ExpressionStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeExpressionStatement(info.Expression);
		}
	}

	public class GotoStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.GotoStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as GotoStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeGotoStatement(info.Label);
		}
	}

	public class IterationStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.IterationStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as IterationStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeIterationStatement(info.InitStatement, info.TestExpression, info.IncrementStatement, info.Statements);
		}
	}

	public class LabeledStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.LabeledStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as LabeledStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeLabeledStatement(info.Label);
		}
	}

	public class RemoveEventStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.RemoveEventStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as RemoveEventStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeRemoveEventStatement(info.EventRef, info.EventName, info.ListenerExpression);
		}
	}

	public class SnippetStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.SnippetStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as SnippetStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeSnippetStatement(info.Value);
		}
	}

	public class ThrowExceptionStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.ThrowExceptionStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as ThrowExceptionStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeThrowExceptionStatement(info.ToThrowExpression);
		}
	}

	public class TryCatchFinallyStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.TryCatchFinallyStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as TryCatchFinallyStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeTryCatchFinallyStatement(info.TryStatements, info.CatchClauses, info.FinallyStatements);
		}
	}

	public class VariableDeclarationStatementHelper : BaseStatementHelper
	{
		public override bool Supports(CodeStatementType codeStatementType)
		{
			return CodeStatementType.VariableDeclarationStatement == codeStatementType;
		}

		public override CodeStatement CreateStatement(IStatementInfo statementInfo)
		{
			var info = statementInfo as VariableDeclarationStatementInfo;
			if (info == null) throw new IncorectStatementInfoException();

			return new CodeVariableDeclarationStatement(info.Type, info.VariableName, info.InitExpression);
		}
	}
}