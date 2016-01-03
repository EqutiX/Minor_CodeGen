using System.CodeDom;

namespace CodeGen
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