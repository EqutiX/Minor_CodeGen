using System.CodeDom;
using System.Linq;

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
		public ClassBuilder(string className)
		{
			_currentClass = new CodeTypeDeclaration( className );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sFieldName"></param>
		private T FindMember<T> ( string sFieldName) where T : CodeTypeMember
		{
		    return _currentClass.Members.Cast<T>().FirstOrDefault(member => member.Name == sFieldName);
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
        public ClassBuilder AddVoidMethode(string name, MemberAttributes attr = MemberAttributes.Public)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();

            var method = CreateCodeMemberMethod(name, attr, new CodeTypeReference(typeof (void)));

            _currentClass.Members.Add(method);
            return this;
        }
	}
}
