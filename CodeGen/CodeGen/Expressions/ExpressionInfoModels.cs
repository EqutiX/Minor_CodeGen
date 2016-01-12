using System.CodeDom;

namespace CodeGen.Expressions
{
	public class Iets
	{
		void iets()
		{
			//new CodeArgumentReferenceExpression(string ParameterName)
			//new CodeArrayCreateExpression(CodeTypeReference CreateType, CodeExpression Size | params CodeExpression[] Initializers)
			//new CodeArrayIndexerExpression(CodeExpression TargetObject, params CodeExpression[] Indices)
			//new CodeBaseReferenceExpression()
			//new CodeBinaryOperatorExpression(CodeExpression Left, CodeBinaryOperatorType Op, CodeExpression Right)
			//new CodeCastExpression(CodeTypeReference TargetType, CodeExpression Expression)
			//new CodeDefaultValueExpression(CodeTypeReference Type)
			//new CodeDelegateCreateExpression(CodeTypeReference DelegateType, CodeExpression TargetObject, string MethodName)
			//new CodeDelegateInvokeExpression(CodeExpression TargetObject, params CodeExpression[] Parameters)
			//new CodeDirectionExpression(FieldDirection Direction, CodeExpression Expression)
			//new CodeEventReferenceExpression(CodeExpression TargetObject, string EventName)
			//new CodeFieldReferenceExpression(CodeExpression TargetObject, string FieldName)
			//new CodeIndexerExpression(CodeExpression TargetObject, params CodeExpression[] Indices)
			//new CodeMethodInvokeExpression(CodeMethodReferenceExpression Method | (CodeExpression TargetObject, string MethodName), params CodeExpression[] Parameters)
			//new CodeMethodReferenceExpression(CodeExpression TargetObject, string MethodName, params CodeTypeReference[] TypeParameters)
			//new CodeObjectCreateExpression(CodeTypeReference CreateType, params CodeExpression[] Parameters)
			//new CodeParameterDeclarationExpression(CodeTypeReference Type, string Name)
			//new CodePrimitiveExpression(object Value)
			//new CodePropertyReferenceExpression(CodeExpression TargetObject, string PropertyName)
			//new CodePropertySetValueReferenceExpression()
			//new CodeSnippetExpression(string Value)
			//new CodeThisReferenceExpression()
			//new CodeTypeOfExpression(CodeTypeReference Type)
			//new CodeTypeReferenceExpression(CodeTypeReference Type)
			//new CodeVariableReferenceExpression(string VariableName)
		}
    }
    public interface IExpressionInfo
    {
    }

	public class ArgumentReferenceExpressionInfo : IExpressionInfo
	{
		public string ParameterName { get; set; }
	}
}
