using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace MyTools
{
    public class BetaClassBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		private CodeTypeDeclaration _currentClass;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		public BetaClassBuilder(string className)
		{
			_currentClass = new CodeTypeDeclaration( className );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFieldName"></param>
		private T FindMember<T> ( string sFieldName) where T : CodeTypeMember
		{
			foreach(T member in _currentClass.Members)
			{
				if(member.Name == sFieldName) return member;
			}
			return null;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFieldName"></param>
		/// <returns></returns>
		public BetaClassBuilder AddField<T>( string sFieldName , MemberAttributes attr = MemberAttributes.Public) 
		{
			var field = FindMember<CodeMemberField>(sFieldName) ?? CreateCodeMemberField<T>( sFieldName, attr);
			return this;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sFieldName"></param>
		/// <returns></returns>
		private CodeMemberField CreateCodeMemberField<T>( string sFieldName, MemberAttributes attr)
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = attr;
			field.Name = sFieldName;
			field.Type = new CodeTypeReference( typeof( T ) );
			_currentClass.Members.Add( field );
			return field;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public CodeTypeDeclaration GetDeclaration()
		{
			return _currentClass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public BetaClassBuilder AddFieldValue<T>( string sFieldName, T oFieldValue )
		{
			var member = FindMember<CodeMemberField>( sFieldName );

			if( member == null )
				throw new FieldNotFoundException();

			member.InitExpression = new CodePrimitiveExpression( oFieldValue );

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public BetaClassBuilder AddFunction()
		{
			// Declaring a ToString method
			CodeMemberMethod toStringMethod = new CodeMemberMethod();
			toStringMethod.Attributes =
				MemberAttributes.Public | MemberAttributes.Override;
			toStringMethod.Name = "ToString";
			toStringMethod.ReturnType =
				new CodeTypeReference( typeof( System.String ) );

			CodeFieldReferenceExpression widthReference =
				new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "Width" );
			CodeFieldReferenceExpression heightReference =
				new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "Height" );
			CodeFieldReferenceExpression areaReference =
				new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), "Area" );

			// Declaring a return statement for method ToString.
			CodeMethodReturnStatement returnStatement =
				new CodeMethodReturnStatement();

			// This statement returns a string representation of the width,
			// height, and area.
			string formattedOutput = "The object:" + Environment.NewLine +
				" width = {0}," + Environment.NewLine +
				" height = {1}," + Environment.NewLine +
				" area = {2}";
			returnStatement.Expression =
				new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression( "System.String" ), "Format",
				new CodePrimitiveExpression( formattedOutput ),
				widthReference, heightReference, areaReference );
			toStringMethod.Statements.Add( returnStatement );
			_currentClass.Members.Add( toStringMethod );
			return this;
		}
	}
}
