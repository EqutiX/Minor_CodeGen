using System;
using System.CodeDom;
using System.Linq;

namespace CodeGen
{
    public interface IExpressionLine
    {

        CodeExpression CreateExpression();
    }
}

namespace CodeGen.Expressions
{
    public class ArgumentReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeArgumentReferenceExpression(ParameterName);
        }

        public string ParameterName { get; set; }
    }

    public class ArrayCreateExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            var createType = new CodeTypeReference(Type);
            return Initializers != null
                ? new CodeArrayCreateExpression(createType, Initializers.Select(i => i.CreateExpression()).ToArray())
                : new CodeArrayCreateExpression(createType, Size);
        }

        public int Size { get; set; }

        public IExpressionLine[] Initializers { get; set; }

        public Type Type { get; set; }
    }

    public class CodeArrayIndexerExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeArrayIndexerExpression(TargetObject.CreateExpression(),
                Indices.Select(i => i.CreateExpression()).ToArray());
        }

        public IExpressionLine[] Indices { get; set; }

        public IExpressionLine TargetObject { get; set; }
    }

    public class BaseReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeBaseReferenceExpression();
        }
    }

    public class BinaryOperatorExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            CodeBinaryOperatorType result;
            if (Enum.TryParse(OperatorType, out result))
                return new CodeBinaryOperatorExpression(Left.CreateExpression(), result, Rights.CreateExpression());
            throw new Exception("OperatorType Not Found.");
        }

        public string OperatorType { get; set; }

        public IExpressionLine Rights { get; set; }

        public IExpressionLine Left { get; set; }
    }

    public class CastExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeCastExpression(TargetType, Expression.CreateExpression());
        }

        public IExpressionLine Expression { get; set; }

        public Type TargetType { get; set; }
    }

    public class DefaultValueExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            var type = new CodeTypeReference(Type);
            return new CodeDefaultValueExpression(type);
        }

        public Type Type { get; set; }
    }

    public class DelegateCreateExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            var delegateType = new CodeTypeReference(Type);
            return new CodeDelegateCreateExpression(delegateType, TargetObject.CreateExpression(), MethodeName);
        }

        public Type Type { get; set; }

        public IExpressionLine TargetObject { get; set; }

        public string MethodeName { get; set; }
    }

    public class DelegateInvokeExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return Parameters != null
                ? new CodeDelegateInvokeExpression(TargetObject.CreateExpression(),
                    Parameters.Select(p => p.CreateExpression()).ToArray())
                : new CodeDelegateInvokeExpression(TargetObject.CreateExpression());
        }

        public IExpressionLine[] Parameters { get; set; }

        public IExpressionLine TargetObject { get; set; }
    }

    public class DirectionExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            FieldDirection direction;
            if (Enum.TryParse(Direction, out direction))
                return new CodeDirectionExpression(direction, Expression.CreateExpression());
            throw new Exception("FieldDirection Not Found.");
        }

        public string Direction { get; set; }

        public IExpressionLine Expression { get; set; }
    }

    public class EventReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeEventReferenceExpression(TargetObject.CreateExpression(), EventName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string EventName { get; set; }
    }

    public class FieldReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeFieldReferenceExpression(TargetObject.CreateExpression(), FieldName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string FieldName { get; set; }
    }

    public class IndexerExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeIndexerExpression(TargetObject.CreateExpression(),
                Indices.Select(i => i.CreateExpression()).ToArray());
        }

        public IExpressionLine[] Indices { get; set; }

        public IExpressionLine TargetObject { get; set; }
    }

    public class MethodInvokeExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {

            return new CodeMethodInvokeExpression(TargetObject.CreateExpression(), MethodName,
                Parameters.Select(p => p.CreateExpression()).ToArray());
        }

        public IExpressionLine[] Parameters { get; set; }

        public string MethodName { get; set; }

        public IExpressionLine TargetObject { get; set; }
    }

    public class MethodReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            if (Types == null) return new CodeMethodReferenceExpression(TargetObject.CreateExpression(), MethodName);
            var typeParameters = Types.Select(t => new CodeTypeReference(t)).ToArray();
            return new CodeMethodReferenceExpression(TargetObject.CreateExpression(), MethodName, typeParameters);
        }

        public string MethodName { get; set; }

        public IExpressionLine TargetObject { get; set; }

        public Type[] Types { get; set; }
    }

    public class ObjectCreateExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeObjectCreateExpression(CreateType, Parameters.Select(p => p.CreateExpression()).ToArray());
        }

        public string CreateType { get; set; }

        public IExpressionLine[] Parameters { get; set; }
    }

    public class ParameterDeclarationExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeParameterDeclarationExpression(Type, Name);
        }

        public string Type { get; set; }

        public string Name { get; set; }
    }

    public class PrimitiveExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }

        public object Value { get; set; }
    }

    public class PropertyReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodePropertyReferenceExpression(TargetObject.CreateExpression(), PropertyName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string PropertyName { get; set; }
    }

    public class PropertySetValueReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodePropertySetValueReferenceExpression();
        }
    }

    public class SnippetExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeSnippetExpression(Value);
        }

        public string Value { get; set; }
    }

    public class ThisReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeThisReferenceExpression();
        }
    }

    public class TypeOfExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeTypeOfExpression(Type);
        }

        public string Type { get; set; }
    }

    public class TypeReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeTypeReferenceExpression(Type);
        }

        public string Type { get; set; }
    }

    public class VariableReferenceExpressionLine : IExpressionLine
    {
        public CodeExpression CreateExpression()
        {
            return new CodeVariableReferenceExpression(VariableName);
        }

        public string VariableName { get; set; }
    }
}
