using System.CodeDom;

namespace CodeGen.Expressions
{
    public abstract class BaseExpressionHelper
    {
        public abstract bool Supports(CodeExpressionType codeExpressionType);

        public abstract CodeExpression CreateStatement(IExpressionInfo expressionInfo);
    }
}
