using System;
using System.CodeDom;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace CodeGen
{
    public class ClassBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		private readonly CodeTypeDeclaration _currentClass;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		public ClassBuilder(string className, TypeAttributes attr = TypeAttributes.Public, bool isStatic = false)
		{
			_currentClass = new CodeTypeDeclaration((isStatic? "static ": "")+className );
			_currentClass.IsClass = true;
            _currentClass.TypeAttributes = attr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFieldName"></param>
		private T FindMember<T> ( string sFieldName) where T : CodeTypeMember
		{
		    return _currentClass.Members.OfType<T>().FirstOrDefault(member => member.Name == sFieldName);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFieldName"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public ClassBuilder AddField<T>( string sFieldName , MemberAttributes attr = MemberAttributes.Public)
        {
           if( FindMember<CodeMemberField>(sFieldName) != null)
                throw new FieldAlreadyExistsException();
            CreateCodeMemberField<T>( sFieldName, attr);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sFieldName"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private void CreateCodeMemberField<T>(string sFieldName, MemberAttributes attr)
		{
            var field = new CodeMemberField
            {
                Attributes = attr,
                Name = sFieldName,
                Type = new CodeTypeReference(typeof (T))
            };
            _currentClass.Members.Add( field );
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
		public ClassBuilder AddFieldValue<T>( string sFieldName, T oFieldValue )
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
		public ClassBuilder AddMethod<T>(string name, MemberAttributes attr = MemberAttributes.Public)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();
			
		    var method = CreateCodeMemberMethod(name, attr,new CodeTypeReference(typeof(T)));

            var returnStatement = new CodeMethodReturnStatement();
            method.Statements.Add( returnStatement );

            _currentClass.Members.Add( method );
			return this;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attr"></param>
        /// <param name="codeTypeReference"></param>
        /// <returns></returns>
        private static CodeMemberMethod CreateCodeMemberMethod(string name, MemberAttributes attr, CodeTypeReference codeTypeReference)
        {
            return new CodeMemberMethod
            {
                Attributes = attr,
                Name = name,
                ReturnType = codeTypeReference
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public ClassBuilder AddVoidMethod(string name, MemberAttributes attr = MemberAttributes.Public, string[] lines = null)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();

            var method = CreateCodeMemberMethod(name, attr, new CodeTypeReference(typeof (void)));
			CodePrimitiveExpression expr = new CodePrimitiveExpression(lines[0]);
			CodeVariableDeclarationStatement decl1 = new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(10));
			method.Statements.Add(decl1);
			_currentClass.Members.Add(method);
            return this;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sPropertyName"></param>
		/// <param name="attr"></param>
		/// <returns></returns>
		public ClassBuilder AddProperty<T>(string sPropertyName, MemberAttributes attr = MemberAttributes.Private)
		{
            if (FindMember<CodeMemberProperty>(sPropertyName) != null)
				throw new PropertyAlreadyExistsException();
			CreateCodeMemberProperty<T>(sPropertyName, attr);
			return this;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sPropertyName"></param>
		/// <param name="attr"></param>
		/// <returns></returns>
		private void CreateCodeMemberProperty<T>(string sPropertyName, MemberAttributes attr)
		{
			var Property = new CodeMemberProperty
			{
				Attributes = attr,
				Name = sPropertyName,
				Type = new CodeTypeReference(typeof(T)),
				HasGet = true,
				HasSet = true
			};
			Property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), sPropertyName+"Value")));
			Property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), sPropertyName + "Value"), new CodePropertySetValueReferenceExpression()));
			_currentClass.Members.Add(Property);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ClassBuilder AddPropertyValue<T>(string sPropertyName, T oPropertyValue)
		{
			/*var member = FindMember<CodeMemberProperty>(sPropertyName);

			if (member == null)
				throw new PropertyNotFoundException();

			member.InitExpression = new CodePrimitiveExpression(oPropertyValue);*/

			return this;
		}
	}
}
