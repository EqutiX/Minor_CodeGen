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
}