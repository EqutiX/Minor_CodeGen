using System.Collections.Generic;
using System.Linq;

namespace CodeGen.Statements
{
	public enum CodeStatementType
    {
        ReturnStatement,
        CommentStatement,
		AssignStatement,
		AttachEventStatement,
		ConditionStatement,
		ExpressionStatement,
		GotoStatement,
		IterationStatement,
		LabeledStatement,
		RemoveEventStatement,
		SnippetStatement,
		ThrowExceptionStatement,
		TryCatchFinallyStatement,
		VariableDeclarationStatement
	}

    public class CodeStatementService
    {
        private static readonly List<BaseStatementHelper> Helpers = new List<BaseStatementHelper>
        {
            new CommentStatementHelper(),
            new ReturnStatementHelper(),
            new AssignStatementHelper(),
			new AttachEventStatementHelper(),
			new ConditionStatementHelper(),
			new ExpressionStatementHelper(),
			new GotoStatementHelper(),
			new IterationStatementHelper(),
			new LabeledStatementHelper(),
			new RemoveEventStatementHelper(),
			new SnippetStatementHelper(),
			new ThrowExceptionStatementHelper(),
			new TryCatchFinallyStatementHelper(),
			new VariableDeclarationStatementHelper()
		};  

        public  static BaseStatementHelper GetStatementHelper(CodeStatementType codeStatementType)
        {
            return Helpers.SingleOrDefault(h => h.Supports(codeStatementType));
        }
    }
}
