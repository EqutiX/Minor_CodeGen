using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeGen
{
    public class ClassBuilder
	{
		/// <summary>
		/// _currentClass is the only CodeTypeDeclaration we use in de builder. Its a readonly because it only can be set in the constructor. 
		/// </summary>
		private readonly CodeTypeDeclaration _currentClass;
		
		/// <summary>
        /// The Construtor initializes _currentClass with the given class name.
		/// </summary>
        /// <param name="className">Name you want the class to have.</param>
        /// <param name="attr">Attributes the class haves.</param>
        /// <param name="isStatic">The make a class static or not. (DEFAULT IS FALSE)</param>
		/// <param name="isInterface">The make a class an interface or not. (DEFAULT IS FALSE)</param>
		public ClassBuilder(string className, TypeAttributes attr = TypeAttributes.Public, bool isStatic = false, string sBaseClass = "")
		{
			_currentClass = new CodeTypeDeclaration( );
		    _currentClass = new CodeTypeDeclaration((isStatic ? "static " : "") + className)
		    {
		        IsClass = true,
				TypeAttributes = attr,
		    };
			if (!string.IsNullOrWhiteSpace(sBaseClass))
			{
				_currentClass.BaseTypes.Add(new CodeTypeReference(sBaseClass));
			}
		}

		/// <summary>
        /// SetConstructor Sets the construcor of the class you are making.
		/// </summary>
        /// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
        /// <param name="lines">An Array of strings to define the code-lines of the method.</param>
        /// <param name="attr">MemberAttributes to define the Attributes of the method.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder SetConstructor(ParameterItem[]parameterItems,string[] lines, MemberAttributes attr = MemberAttributes.Public)
        {
            var constructor = new CodeConstructor {Attributes = attr};

            (parameterItems.ToList()).ForEach(
                i => constructor.Parameters.Add(new CodeParameterDeclarationExpression(i.Type, i.Name)));

            _currentClass.Members.Add(constructor);

            return this;
        }

		/// <summary>
        /// FindMembers find members of given Type and with given field name.
		/// </summary>
        /// <typeparam name="T">Type you want to find needs to be a type of CodeTypeMember.</typeparam>
        /// <param name="name">Name of member you want to find.</param>
        /// <returns>Found member or null.</returns>
        private T FindMember<T> ( string name) where T : CodeTypeMember
		{
		    return _currentClass.Members.OfType<T>().FirstOrDefault(member => member.Name == name);
		}
		
        /// <summary>
        /// AddField adds a new CodeMemberField to the CodeTypeDeclaration with the given name and attributes.
        /// </summary>
        /// <typeparam name="T">Type of field you want to add.</typeparam>
        /// <param name="fieldName">Name of of the field you want to add.</param>
        /// <param name="attr">Attribute you want the field to have.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddField<T>( string fieldName , MemberAttributes attr = MemberAttributes.Public)
        {
           if( FindMember<CodeMemberField>(fieldName) != null)
                throw new FieldAlreadyExistsException();
            CreateCodeMemberField<T>( fieldName, attr);
            return this;
        }
		
        /// <summary>
        /// CreateCodeMemberField Creates a CodeMemberField and adds it to the _currentClass members.
        /// </summary>
        /// <typeparam name="T">Type of field that is going to be added.</typeparam>
        /// <param name="fieldName">Name of the field that is going to be added.</param>
        /// <param name="attr">Attributes of the field that is going to be added. </param>
        private void CreateCodeMemberField<T>(string fieldName, MemberAttributes attr)
		{
            var field = new CodeMemberField
            {
                Attributes = attr,
                Name = fieldName,
                Type = new CodeTypeReference(typeof (T))
            };
			
            _currentClass.Members.Add( field );
		}

		/// <summary>
        /// Returns the CodeTypeDeclaration used in the builder.
		/// </summary>
        /// <returns>CodeTypeDeclaration used in the builder</returns>
		public CodeTypeDeclaration GetDeclaration()
		{
			return _currentClass;
		}

		/// <summary>
        /// AddFieldValue adds a value to an already created field.
		/// </summary>
        /// <typeparam name="T">Type of the field that is going to be used.</typeparam>
        /// <param name="fieldName">Name of the field that is going to be used.</param>
        /// <param name="fieldValue">Value that is going to be set.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddFieldValue<T>( string fieldName, T fieldValue )
		{
			var member = FindMember<CodeMemberField>( fieldName );

			if( member == null )
				throw new FieldNotFoundException();

			member.InitExpression = new CodePrimitiveExpression( fieldValue );

			return this;
		}

        /// <summary>
        /// AddNethod adds a method to the CodeTypeDeclaration.
        /// </summary>
        /// <typeparam name="T">Type of method that will be added.</typeparam>
        /// <param name="name">Name of the method that will be added.</param>
        /// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
        /// <param name="attr">Attributes of the method that will be added.</param>
        /// <param name="lines">An Array of strings to define the code-lines of the method.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddMethod<T>(string name, ParameterItem[] parameterItems, MemberAttributes attr = MemberAttributes.Public, IStatementLine[] lines = null)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();
			
		    var method = CreateCodeMemberMethod(name, attr,new CodeTypeReference(typeof(T)),parameterItems,lines);

            //var returnStatement = new CodeMethodReturnStatement();
			
            //method.Statements.Add( returnStatement );

            _currentClass.Members.Add( method );
			return this;
		}

        /// <summary>
        /// CreateCodeMemberMethod creates a CodeMemberMethod.
        /// </summary>
        /// <param name="name">Name of the method that is going to be created.</param>
        /// <param name="attr">Attributes of the method that is going to be created.</param>
        /// <param name="codeTypeReference">Return type of the method that is going to be created.</param>
        /// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
        /// <param name="lines">An Array of strings to define the code-lines of the method.</param>
        /// <returns>A new CodeMemberMethod that can be added to CodeTypeDeclaration members.</returns>
        private static CodeMemberMethod CreateCodeMemberMethod(string name, MemberAttributes attr, CodeTypeReference codeTypeReference, IEnumerable<ParameterItem> parameterItems, IEnumerable<IStatementLine> lines)
        {
            var codeMemberMethod =  new CodeMemberMethod
            {
                Attributes = attr,
                Name = name,
                ReturnType = codeTypeReference
            };

            (parameterItems.ToList()).ForEach(
                i =>
                    codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                        new CodeTypeReference(i.Type), i.Name)));

            lines?.ToList().ForEach(l => codeMemberMethod.Statements.Add(l.CreateStatement()));


            return codeMemberMethod;
        }

        /// <summary>
        /// AddVoidMethod adds a void method to the CodeTypeDeclaration.
        /// </summary>
        /// <param name="name">Name of the mehtod that will be added.</param>
        /// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
        /// <param name="attr">Attributes of the method that will be added.</param>
        /// <param name="lines">An Array of strings to define the code-lines of the method.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddVoidMethod(string name, ParameterItem[] parameterItems, MemberAttributes attr = MemberAttributes.Public, IStatementLine[] lines = null)
        {
            if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();

            var method = CreateCodeMemberMethod(name, attr, new CodeTypeReference(typeof (void)),parameterItems, lines);
			var declStatment = new CodeVariableDeclarationStatement(
				typeof(int), "i", new CodePrimitiveExpression(10));
			method.Statements.Add(declStatment);
            
          
            _currentClass.Members.Add(method);
            return this;
        }

		/// <summary>
        /// AddProperty adds a property to the CodeTypeDeclaration.
		/// </summary>
        /// <typeparam name="T">Type of property that is going to be added.</typeparam>
        /// <param name="propertyName">Name of the property that is going to be added.</param>
        /// <param name="attr">Attributes of the property that is going to be added.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddProperty<T>(string propertyName, MemberAttributes attr = MemberAttributes.Private)
		{
            if (FindMember<CodeMemberProperty>(propertyName) != null)
				throw new PropertyAlreadyExistsException();
			CreateCodeMemberProperty<T>(propertyName, attr);
			return this;
		}
		
		/// <summary>
        /// CreateCodeMemberProperty creates a proptery and adds it to the CodeTypeDeclaration.
		/// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="attr">Attributes of the property.</param>
        private void CreateCodeMemberProperty<T>(string propertyName, MemberAttributes attr)
		{
			var property = new CodeMemberProperty
			{
				Attributes = attr,
				Name = propertyName,
				Type = new CodeTypeReference(typeof(T)),
				HasGet = true,
				HasSet = true
			};
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyName+"Value")));
			property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyName + "Value"), new CodePropertySetValueReferenceExpression()));
			_currentClass.Members.Add(property);

			var field = AddField<T>(propertyName + "Value", MemberAttributes.Private);
        }

		/// <summary>
        /// AddPropertyValue adds a value to an already created property.
		/// </summary>
        /// <typeparam name="T">Type of property that is going to be used.</typeparam>
        /// <param name="propertyName">Name of the property that is going to be used.</param>
        /// <param name="propertyValue">Value that is going to be set.</param>
        /// <returns>The current ClassBuilder (this)</returns>
        public ClassBuilder AddPropertyValue<T>(string propertyName, T propertyValue)
		{
            //todo: ???
			/*var member = FindMember<CodeMemberProperty>(sPropertyName);

			if (member == null)
				throw new PropertyNotFoundException();

			member.InitExpression = new CodePrimitiveExpression(oPropertyValue);*/

			return this;
		}

		/// <summary>
		/// Adds an entrypoint to the current class
		/// </summary>
		/// <typeparam name="T">return type of the entrypoint</typeparam>
		/// <param name="functionName">Name of the entrypoint function</param>
		/// <returns>The current instance of the ClassBuilder</returns>
		public ClassBuilder AddEntryPoint<T>(string functionName)
		{
		    var start = new CodeEntryPointMethod
		    {
		        Name = functionName,
		        ReturnType = new CodeTypeReference(typeof (T))
		    };
			_currentClass.Members.Add(start);
			return this;
		}
	}
}
