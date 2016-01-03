using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Expressions
{
    public enum CodeExpressionType
    {

    }

    public class CodeStatmentService
    {
        private static readonly List<BaseExpressionHelper> Helpers = new List<BaseExpressionHelper>
        {

        };

        public static BaseExpressionHelper GetStatementHelper(CodeExpressionType codeStatementType)
        {
            return Helpers.SingleOrDefault(h => h.Supports(codeStatementType));
        }
    }
}
