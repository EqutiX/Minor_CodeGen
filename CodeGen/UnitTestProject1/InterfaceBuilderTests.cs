using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGen;

namespace UnitTestProject1
{
	[TestClass]
	public class InterfaceBuilderTests
	{
		const string InterfaceName = "IFirstInterface";
		const string FunctionName = "FirstFunction";
		const string FirstPropertyName = "FirstPropertyName";

		[TestMethod]
		public void IfNewIntanceOfInstanceBuilderGetDeclarationShouldReturnInterfaceWithGivenName()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			var target = builder.GetDeclaration();

			Assert.AreEqual(target.Name, InterfaceName);
		}

		[TestMethod]
		public void IfAddMethodIsCalledShouldAddNewFunctionWithGivenName()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			var target = builder.AddMethod<string>(FunctionName, new ParameterItem[] { }).GetDeclaration();

			Assert.AreEqual(1, target.Members.Count);
			var member = target.Members[0];
			Assert.AreEqual(FunctionName, member.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(MethodAlreadyExistsException))]
		public void IfAddMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			builder.AddMethod<string>(FunctionName, new ParameterItem[] { }).AddMethod<string>(FunctionName, new ParameterItem[] { }).GetDeclaration();
		}

		[TestMethod]
		public void IfAddVoidMethodIsCalledANewMethodShouldBeAddedToTheDeclarationMembers()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			var target = builder.AddVoidMethod(FunctionName, new ParameterItem[] { }).GetDeclaration();

			Assert.AreEqual(1, target.Members.Count);
			Assert.AreEqual(FunctionName, target.Members[0].Name);
		}

		[TestMethod]
		[ExpectedException(typeof(MethodAlreadyExistsException))]
		public void IfAddVoidMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			builder.AddVoidMethod(FunctionName, new ParameterItem[] { }).AddVoidMethod(FunctionName, new ParameterItem[] { }).GetDeclaration();
		}

		[TestMethod]
		public void IfAddPropertyIsCalledShouldAddPropertyToDeclaration()
		{
			var builder = new InterfaceBuilder(InterfaceName);
			var target = builder.AddProperty<string>(FirstPropertyName).GetDeclaration();

			Assert.AreEqual(1, target.Members.Count);
			var member = target.Members[0];
			Assert.AreEqual(FirstPropertyName, member.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(PropertyAlreadyExistsException))]
		public void IfAddPropertyIsCalledTwiceWithTheSamePropertyNameShouldThrowException()
		{
			var builder = new InterfaceBuilder(InterfaceName);

			builder.AddProperty<string>(FirstPropertyName).AddProperty<string>(FirstPropertyName).GetDeclaration();
		}
	}
}
