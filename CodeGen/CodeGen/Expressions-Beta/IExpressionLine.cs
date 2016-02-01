using System.CodeDom;

namespace CodeGen
{
    public interface IExpressionLine
    {

        CodeExpression CreateExpression();
    }
}
