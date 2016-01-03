using System.Collections.Generic;
using System.Linq;

namespace CodeGen.Expressions
{
    public enum CodeExpressionType
    {

    }

    public class CodeExpressionService
    {
        private static readonly List<BaseExpressionHelper> Helpers = new List<BaseExpressionHelper>
        {

        };

        public static BaseExpressionHelper GetExpressionHelper(CodeExpressionType codeStatementType)
        {
            return Helpers.SingleOrDefault(h => h.Supports(codeStatementType));
        }
    }
}
