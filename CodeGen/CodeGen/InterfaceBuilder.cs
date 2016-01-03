using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.Reflection;

namespace CodeGen
{
	public class InterfaceBuilder
	{
		/// <summary>
		/// _currentClass is the only CodeTypeDeclaration we use in de builder. Its a readonly because it only can be set in the constructor. 
		/// </summary>
		private readonly CodeTypeDeclaration _currentInterface;

		/// <summary>
		/// The Construtor initializes _currentClass with the given class name.
		/// </summary>
		/// <param name="className">Name you want the class to have.</param>
		/// <param name="attr">Attributes the class haves.</param>
		/// <param name="isStatic">The make a class static or not. (DEFAULT IS FALSE)</param>
		/// <param name="isInterface">The make a class an interface or not. (DEFAULT IS FALSE)</param>
		public InterfaceBuilder(string className, TypeAttributes attr = TypeAttributes.Public)
		{
			_currentInterface = new CodeTypeDeclaration(className)
			{
				IsClass = false,
				IsInterface = true,
				TypeAttributes = TypeAttributes.Interface | attr
			};
		}

		/// <summary>
		/// SetConstructor Sets the construcor of the class you are making.
		/// </summary>
		/// <param name="parameterItems">An Array of Type/Name combinations to define what the parameters are.</param>
		/// <param name="lines">An Array of strings to define the code-lines of the method.</param>
		/// <param name="attr">MemberAttributes to define the Attributes of the method.</param>
		/// <returns>The current ClassBuilder (this)</returns>
		public InterfaceBuilder SetConstructor(ParameterItem[] parameterItems, string[] lines, MemberAttributes attr = MemberAttributes.Public)
		{
			var constructor = new CodeConstructor { Attributes = attr };

			(parameterItems.ToList()).ForEach(
				i => constructor.Parameters.Add(new CodeParameterDeclarationExpression(i.Type, i.Name)));

			_currentInterface.Members.Add(constructor);

			return this;
		}
	}
}
