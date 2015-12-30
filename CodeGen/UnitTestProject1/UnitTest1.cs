using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGen;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void IfNewIntanceOfBetaBuilderGetDeclarationShouldReturnClassWithGivenName()
		{
			const string className = "FirstClass";
			var builder = new BetaClassBuilder( className );

			var target = builder.GetDeclaration();

			Assert.AreEqual( target.Name, className );
		}

		[TestMethod]
		public void IfAddFieldIsCalledShouldAddFieldToDeclaration()
		{
			const string className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			const string firstFieldName = "FirstField";

			var target = builder.AddField<string>( firstFieldName ).GetDeclaration();

			Assert.AreEqual( 1, target.Members.Count );
			var member = target.Members[0];
			Assert.AreEqual( firstFieldName, member.Name );
		}

        [TestMethod]
        [ExpectedException(typeof(FieldAlreadyExistsException))]
        public void IfAddFieldIsCalledTwiceWithTheSameFieldNameShouldThrowException()
        {
            const string className = "FirstClass";
            var builder = new BetaClassBuilder(className);
            const string firstFieldName = "FirstField";

            builder.AddField<string>(firstFieldName).AddField<string>(firstFieldName).GetDeclaration();

        }

        [TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void IfAddFieldValueToNonExistingFieldShouldThrowException()
		{
			const string className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			const string firstFieldName = "FirstField";
			const string firstFieldValue = "Value";

			builder.AddFieldValue( firstFieldName, firstFieldValue ).GetDeclaration();

		}

		[TestMethod]
		public void IfAddFieldValueToExistingFieldShouldNotThrowException()
		{
			const string className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			const string firstFieldName = "FirstField";
			const string firstFieldValue = "Value";

			var target = builder.AddField<string>( firstFieldName ).AddFieldValue( firstFieldName, firstFieldValue ).GetDeclaration();
			Assert.IsNotNull( target );
		}

		[TestMethod]
		public void IfAddMethodIsCalledShouldAddNewFunctionWithGivenName()
		{
			const string className = "FirstClass";
			var builder = new BetaClassBuilder( className );
			const string functionName = "FirstFunction";

			var target = builder.AddMethod<string>(functionName).GetDeclaration();

            Assert.AreEqual(1, target.Members.Count);
            var member = target.Members[0];
            Assert.AreEqual(functionName, member.Name);
        }

	    [TestMethod]
	    [ExpectedException(typeof(MethodAlreadyExistsException))]
	    public void IfAddMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
	    {
            const string className = "FirstClass";
            var builder = new BetaClassBuilder(className);
            const string functionName = "FirstFunction";

            builder.AddMethod<string>(functionName).AddMethod<string>(functionName).GetDeclaration();
        }
	}
}
