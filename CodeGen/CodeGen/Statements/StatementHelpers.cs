using System.CodeDom;

namespace CodeGen
{
    public abstract class BaseStatementHelper
    {
        public abstract bool Supports(CodeStatementType codeStatementType);

        public abstract CodeStatement CreateStatement(IStatementInfo statementInfo);
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

    public class ReturnStatmentHelper : BaseStatementHelper
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
}