using System.Collections.Generic;
using System.Linq;

namespace CodeGen.Statements
{
    public enum CodeStatementType
    {
        ReturnStatement,
        CommentStatement
    }
    public class CodeStatmentService
    {
        private static readonly List<BaseStatementHelper> Helpers = new List<BaseStatementHelper>
        {
            new CommentStatementHelper(),
            new ReturnStatmentHelper()
        };  

        public  static BaseStatementHelper GetStatementHelper(CodeStatementType codeStatementType)
        {
            return Helpers.SingleOrDefault(h => h.Supports(codeStatementType));
        }
    }

    
}
