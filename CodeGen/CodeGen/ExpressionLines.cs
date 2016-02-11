using System;
using System.CodeDom;
using System.Linq;

namespace CodeGen
{

    /// <summary>
    /// IExpressionLine is an Interface used to define a ExpressionLine class.
    /// </summary>
    public interface IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        CodeExpression CreateExpression();
    }
}

namespace CodeGen.Expressions
{
    /// <summary>
    /// ArgumentReferenceExpressionLine is a class for simply creating a Argument Expression.
    /// </summary>
    public class ArgumentReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeArgumentReferenceExpression(ParameterName);
        }

        /// <summary>
        /// The name of the paramter to reference.
        /// </summary>
        public string ParameterName { get; set; }
    }

    /// <summary>
    /// ArrayCreateExpressionLine is a class for simply creating a Array create Expression.
    /// </summary>
    public class ArrayCreateExpressionLine : IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        public CodeExpression CreateExpression()
        {
            var createType = new CodeTypeReference(Type);
            return Initializers != null
                ? new CodeArrayCreateExpression(createType, Initializers.Select(i => i.CreateExpression()).ToArray())
                : new CodeArrayCreateExpression(createType, Size);
        }

        /// <summary>
        /// The number of indexes of the array.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// An Array of IExpressionLines used to intialize the array.
        /// </summary>
        public IExpressionLine[] Initializers { get; set; }

        /// <summary>
        /// Type that indicates of the array to create.
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// CodeArrayIndexerExpressionLine is a class for simply creating a Array Indexer Expression.
    /// </summary>
    public class CodeArrayIndexerExpressionLine : IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeArrayIndexerExpression(TargetObject.CreateExpression(),
                Indices.Select(i => i.CreateExpression()).ToArray());
        }

        /// <summary>
        /// Indexes to reference.
        /// </summary>
        public IExpressionLine[] Indices { get; set; }

        /// <summary>
        /// An IExpressionLine that indicates the indexer targets.
        /// </summary>
        public IExpressionLine TargetObject { get; set; }
    }

    /// <summary>
    /// BaseReferenceExpressionLine is a class for simply creating a Base Reference Expression.
    /// </summary>
    public class BaseReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeBaseReferenceExpression();
        }
    }

    /// <summary>
    /// BinaryOperatorExpressionLine is a class for simply creating a Binary operator Expression.
    /// </summary>
    public class BinaryOperatorExpressionLine : IExpressionLine
    {
        /// <summary>
        /// CreateExpression generates a CodeExpression based on the given member data.
        /// </summary>
        /// <returns>Generated CodeExpression.</returns>
        public CodeExpression CreateExpression()
        {
            CodeBinaryOperatorType result;
            if (Enum.TryParse(OperatorType, out result))
                return new CodeBinaryOperatorExpression(Left.CreateExpression(), result, Right.CreateExpression());
            throw new Exception("OperatorType Not Found.");
        }

        /// <summary>
        /// Type of Operator.
        /// </summary>
        public string OperatorType { get; set; }

        /// <summary>
        ///  IExpressionLine for the Right side of the operator.
        /// </summary>
        public IExpressionLine Right { get; set; }

        /// <summary>
        ///  IExpressionLine for the Left side of the operator.
        /// </summary>
        public IExpressionLine Left { get; set; }
    }

    /// <summary>
    /// CastExpressionLine is a class for simply creating a Cast Expression.
    /// </summary>
    public class CastExpressionLine : IExpressionLine
    {
        /// <summary>
		/// Create a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeCastExpression(TargetType, Expression.CreateExpression());
        }

        /// <summary>
        ///  IExpressionLine to cast.
        /// </summary>
        public IExpressionLine Expression { get; set; }

        /// <summary>
        /// The Destination type of the cast.
        /// </summary>
        public Type TargetType { get; set; }
    }

    /// <summary>
    /// DefaultValueExpressionLine is a class for simply creating a Default Value Expression.
    /// </summary>
    public class DefaultValueExpressionLine : IExpressionLine
    {
        /// <summary>
		/// Create a statementLine using the given member data.
		/// </summary>
		/// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            var type = new CodeTypeReference(Type);
            return new CodeDefaultValueExpression(type);
        }

        /// <summary>
        /// Type that specifies the reference to a value type
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// DelegateCreateExpressionLine is a class for simply creating a Delegate Create Expression.
    /// </summary>
    public class DelegateCreateExpressionLine : IExpressionLine
    {

        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            var delegateType = new CodeTypeReference(Type);
            return new CodeDelegateCreateExpression(delegateType, TargetObject.CreateExpression(), MethodeName);
        }

        /// <summary>
        /// Type that indicates the data type of the delegate.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// An IExpressionLine that indicates the object containing the event-handler method.
        /// </summary>
        public IExpressionLine TargetObject { get; set; }

        /// <summary>
        /// The name of the event-handler method.
        /// </summary>
        public string MethodeName { get; set; }
    }

    /// <summary>
    /// DelegateInvokeExpressionLine is a class for simply creating a Delegate Invoke Expression.
    /// </summary>
    public class DelegateInvokeExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return Parameters != null
                ? new CodeDelegateInvokeExpression(TargetObject.CreateExpression(),
                    Parameters.Select(p => p.CreateExpression()).ToArray())
                : new CodeDelegateInvokeExpression(TargetObject.CreateExpression());
        }

        /// <summary>
        /// Array that indicate the parameters. 
        /// </summary>
        public IExpressionLine[] Parameters { get; set; }

        /// <summary>
        /// Expression that indicates the target object.
        /// </summary>
        public IExpressionLine TargetObject { get; set; }
    }

    /// <summary>
    /// DirectionExpressionLine is a class for simply creating a Direction  Expression.
    /// </summary>
    public class DirectionExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            FieldDirection direction;
            if (Enum.TryParse(Direction, out direction))
                return new CodeDirectionExpression(direction, Expression.CreateExpression());
            throw new Exception("FieldDirection Not Found.");
        }

        /// <summary>
        /// Direction that inid
        /// </summary>
        public string Direction { get; set; }
        
        /// <summary>
        /// IExpression that indicates the code expression to represent.
        /// </summary>
        public IExpressionLine Expression { get; set; }
    }

    public class EventReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeEventReferenceExpression(TargetObject.CreateExpression(), EventName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string EventName { get; set; }
    }

    public class FieldReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeFieldReferenceExpression(TargetObject.CreateExpression(), FieldName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string FieldName { get; set; }
    }

    public class IndexerExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
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
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
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
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
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
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeObjectCreateExpression(CreateType, Parameters.Select(p => p.CreateExpression()).ToArray());
        }

        public string CreateType { get; set; }

        public IExpressionLine[] Parameters { get; set; }
    }

    public class ParameterDeclarationExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeParameterDeclarationExpression(Type, Name);
        }

        public string Type { get; set; }

        public string Name { get; set; }
    }

    public class PrimitiveExpressionLine : IExpressionLine
    {

        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodePrimitiveExpression(Value);
        }

        public object Value { get; set; }
    }

    public class PropertyReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodePropertyReferenceExpression(TargetObject.CreateExpression(), PropertyName);
        }

        public IExpressionLine TargetObject { get; set; }

        public string PropertyName { get; set; }
    }

    public class PropertySetValueReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodePropertySetValueReferenceExpression();
        }
    }

    public class SnippetExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeSnippetExpression(Value);
        }

        public string Value { get; set; }
    }

    public class ThisReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeThisReferenceExpression();
        }
    }

    public class TypeOfExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeTypeOfExpression(Type);
        }

        public string Type { get; set; }
    }

    public class TypeReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeTypeReferenceExpression(Type);
        }

        public string Type { get; set; }
    }

    public class VariableReferenceExpressionLine : IExpressionLine
    {
        /// <summary>
        /// Create a statementLine using the given member data.
        /// </summary>
        /// <returns>Returns the generated CodeStatement.</returns>
        public CodeExpression CreateExpression()
        {
            return new CodeVariableReferenceExpression(VariableName);
        }

        public string VariableName { get; set; }
    }
}
