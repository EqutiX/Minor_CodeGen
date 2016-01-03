using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeGen.Expressions
{
    public abstract class BaseExpressionHelper
    {
        public abstract bool Supports(CodeExpressionType codeExpressionType);

        public abstract CodeExpression CreateStatement(IExpressionInfo expressionInfo);
    }
}
