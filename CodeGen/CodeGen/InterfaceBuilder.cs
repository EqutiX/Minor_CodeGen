using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.Reflection;

namespace CodeGen
{
    /// <summary>
    /// InterFaceBuilder is a Class to simply create an Interface for .NET CodeDom.
    /// </summary>
	public class InterfaceBuilder
	{
		/// <summary>
		/// _currentInterface is the only CodeTypeDeclaration we use in de builder. Its a readonly because it only can be set in the constructor. 
		/// </summary>
		private readonly CodeTypeDeclaration _currentInterface;

		/// <summary>
		/// The Construtor initializes _currentInterface with the given interface name.
		/// </summary>
		/// <param name="interfaceName">Name you want the interface to have.</param>
		/// <param name="attr">Attributes the class haves.</param>
		public InterfaceBuilder(string interfaceName, TypeAttributes attr = TypeAttributes.Public)
		{
			_currentInterface = new CodeTypeDeclaration(interfaceName)
			{
				IsClass = false,
				IsInterface = true,
				TypeAttributes = TypeAttributes.Interface | attr
			};
		}

		/// <summary>
		/// Returns the CodeTypeDeclaration used in the builder.
		/// </summary>
		/// <returns>CodeTypeDeclaration used in the builder</returns>
		public CodeTypeDeclaration GetDeclaration()
		{
			return _currentInterface;
		}

		/// <summary>
		/// FindMembers find members of given Type and with given field name.
		/// </summary>
		/// <typeparam name="T">Type you want to find needs to be a type of CodeTypeMember.</typeparam>
		/// <param name="name">Name of member you want to find.</param>
		/// <returns>Found member or null.</returns>
		private T FindMember<T>(string name) where T : CodeTypeMember
		{
			return _currentInterface.Members.OfType<T>().FirstOrDefault(member => member.Name == name);
		}

		/// <summary>
		/// CreateCodeMemberMethod creates a CodeMemberMethod.
		/// </summary>
		/// <param name="name">Name of the method that is going to be created.</param>
		/// <param name="codeTypeReference">Return type of the method that is going to be created.</param>
		/// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
		/// <returns>A new CodeMemberMethod that can be added to CodeTypeDeclaration members.</returns>
		private static CodeMemberMethod CreateCodeMemberMethod(string name, CodeTypeReference codeTypeReference, ParameterItem[] parameterItems)
		{
			var codeMemberMethod = new CodeMemberMethod
			{
				Name = name,
				ReturnType = codeTypeReference
			};

			(parameterItems.ToList()).ForEach(
				i =>
					codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(
						new CodeTypeReference(i.Type),i.Name)));

			return codeMemberMethod;
		}

		/// <summary>
		/// AddNethod adds a method to the CodeTypeDeclaration.
		/// </summary>
		/// <typeparam name="T">Type of method that will be added.</typeparam>
		/// <param name="name">Name of the method that will be added.</param>
		/// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
		/// <returns>The current InterfaceBuilder (this)</returns>
		public InterfaceBuilder AddMethod<T>(string name, ParameterItem[] parameterItems)
		{
			if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();

			var method = CreateCodeMemberMethod(name, new CodeTypeReference(typeof(T)), parameterItems);
			
			_currentInterface.Members.Add(method);
			return this;
		}

		/// <summary>
		/// AddVoidMethod adds a void method to the CodeTypeDeclaration.
		/// </summary>
		/// <param name="name">Name of the mehtod that will be added.</param>
		/// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
		/// <returns>The current InterfaceBuilder (this)</returns>
		public InterfaceBuilder AddVoidMethod(string name, ParameterItem[] parameterItems)
		{
			if (FindMember<CodeMemberMethod>(name) != null) throw new MethodAlreadyExistsException();

			var method = CreateCodeMemberMethod(name, new CodeTypeReference(typeof(void)), parameterItems);
			_currentInterface.Members.Add(method);
			return this;
		}

		/// <summary>
		/// AddProperty adds a property to the CodeTypeDeclaration.
		/// </summary>
		/// <typeparam name="T">Type of property that is going to be added.</typeparam>
		/// <param name="propertyName">Name of the property that is going to be added.</param>
		/// <returns>The current ClassBuilder (this)</returns>
		public InterfaceBuilder AddProperty<T>(string propertyName)
		{
			if (FindMember<CodeMemberProperty>(propertyName) != null)
				throw new PropertyAlreadyExistsException();
			CreateCodeMemberProperty<T>(propertyName);
			return this;
		}

		/// <summary>
		/// CreateCodeMemberProperty creates a proptery and adds it to the CodeTypeDeclaration.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="propertyName">Name of the property.</param>
		private void CreateCodeMemberProperty<T>(string propertyName)
		{
			var property = new CodeMemberProperty
			{
				Name = propertyName,
				Type = new CodeTypeReference(typeof(T)),
				HasGet = true,
				HasSet = true
			};
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyName + "Value")));
			property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), propertyName + "Value"), new CodePropertySetValueReferenceExpression()));
			_currentInterface.Members.Add(property);
		}
	}
}
