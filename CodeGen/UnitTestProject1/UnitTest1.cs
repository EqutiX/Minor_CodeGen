using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyTools;
using System.Linq;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void IfNewIntanceOfBetaBuilderGetDeclarationShouldReturnClassWithGivenName()
		{
			var className = "FirstClass";
			var builder = new BetaClassBuilder( className );

			var target = builder.GetDeclaration();

			Assert.AreEqual( target.Name, className );
		}

		[TestMethod]
		public void IfAddFieldIsCalledShouldAddFieldToDeclaration()
		{
			var className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			var FirstFieldName = "FirstField";

			var target = builder.AddField<String>( FirstFieldName ).GetDeclaration();

			Assert.AreEqual( 1, target.Members.Count );
			var member = target.Members[0];
			Assert.AreEqual( FirstFieldName, member.Name );
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void IfAddFieldValueToNonExistingFieldShouldThrowException()
		{
			var className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			var FirstFieldName = "FirstField";
			var FirstFieldValue = "Value";

			var target = builder.AddFieldValue<String>( FirstFieldName, FirstFieldValue ).GetDeclaration();

		}

		[TestMethod]
		public void IfAddFieldValueToExistingFieldShouldNotThrowException()
		{
			var className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			var FirstFieldName = "FirstField";
			var FirstFieldValue = "Value";

			var target = builder.AddField<String>( FirstFieldName ).AddFieldValue<String>( FirstFieldName, FirstFieldValue ).GetDeclaration();
			Assert.IsNotNull( target );
		}

		[TestMethod]
		public void IfAddFunctionShouldAddNewFunction()
		{
			var className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			var functionName = "FirstFunction";
			var target = builder.AddFunction();
		}
	}
}
