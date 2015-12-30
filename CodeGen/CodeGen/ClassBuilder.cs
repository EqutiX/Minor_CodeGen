﻿using System.CodeDom;
using System.Linq;

namespace CodeGen
{
    public class BetaClassBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		private readonly CodeTypeDeclaration _currentClass;
		
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
		    return _currentClass.Members.Cast<T>().FirstOrDefault(member => member.Name == sFieldName);
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFieldName"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public BetaClassBuilder AddField<T>( string sFieldName , MemberAttributes attr = MemberAttributes.Public)
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
