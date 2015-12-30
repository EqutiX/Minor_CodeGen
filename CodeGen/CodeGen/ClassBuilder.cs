using System;
using System.CodeDom;

namespace CodeGen
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
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
		public BetaClassBuilder AddMethod<T>(string name, MemberAttributes attr = MemberAttributes.Public)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();
			// Declaring a ToString method
		    var method = new CodeMemberMethod
		    {
		        Attributes = attr,
		        Name = name,
		        ReturnType = new CodeTypeReference(typeof (T))
		    };

            var returnStatement = new CodeMethodReturnStatement();
            method.Statements.Add( returnStatement );

            _currentClass.Members.Add( method );
			return this;
		}
	}
}
