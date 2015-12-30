using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGen;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
        const string ClassName = "FirstClass";

        [TestMethod]
		public void IfNewIntanceOfBetaBuilderGetDeclarationShouldReturnClassWithGivenName()
		{
			
			var builder = new ClassBuilder( ClassName );

			var target = builder.GetDeclaration();

			Assert.AreEqual( target.Name, ClassName );
		}

		[TestMethod]
		public void IfAddFieldIsCalledShouldAddFieldToDeclaration()
		{
			
			var builder = new ClassBuilder( ClassName );
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

            var builder = new ClassBuilder(ClassName);
            const string firstFieldName = "FirstField";

            builder.AddField<string>(firstFieldName).AddField<string>(firstFieldName).GetDeclaration();

        }

        [TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void IfAddFieldValueToNonExistingFieldShouldThrowException()
		{

			var builder = new ClassBuilder( ClassName );
			const string firstFieldName = "FirstField";
			const string firstFieldValue = "Value";

			builder.AddFieldValue( firstFieldName, firstFieldValue ).GetDeclaration();
		}

		[TestMethod]
		public void IfAddFieldValueToExistingFieldShouldNotThrowException()
		{

			var builder = new ClassBuilder( ClassName );
			const string firstFieldName = "FirstField";
			const string firstFieldValue = "Value";

			var target = builder.AddField<string>( firstFieldName ).AddFieldValue( firstFieldName, firstFieldValue ).GetDeclaration();
			Assert.IsNotNull( target );
		}

		[TestMethod]
		public void IfAddMethodIsCalledShouldAddNewFunctionWithGivenName()
		{

			var builder = new ClassBuilder( ClassName );
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

            var builder = new ClassBuilder(ClassName);
            const string functionName = "FirstFunction";

            builder.AddMethod<string>(functionName).AddMethod<string>(functionName).GetDeclaration();
        }

	    [TestMethod]
	    public void IfAddVoidMethodIsCalledANewMethodShouldBeAddedToTheDeclarationMembers()
	    {

            var builder = new ClassBuilder(ClassName);
	        const string functionName = "VoidMethod";

	        var target = builder.AddVoidMethod(functionName).GetDeclaration();

            Assert.AreEqual(1,target.Members.Count);
            Assert.AreEqual(functionName, target.Members[0].Name);
	    }

        [TestMethod]
        [ExpectedException(typeof(MethodAlreadyExistsException))]
        public void IfAddVoidMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
        {
            var builder = new ClassBuilder(ClassName);
            const string functionName = "FirstFunction";
					
            builder.AddVoidMethod(functionName).AddVoidMethod(functionName).GetDeclaration();
        }

		[TestMethod]
		public void IfAddPropertyIsCalledShouldAddPropertyToDeclaration()
		{
			var builder = new ClassBuilder(ClassName);
			const string firstPropertyName = "FirstProperty";

			var target = builder.AddProperty<string>(firstPropertyName).GetDeclaration();

			Assert.AreEqual(1, target.Members.Count);
			var member = target.Members[0];
			Assert.AreEqual(firstPropertyName, member.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(PropertyAlreadyExistsException))]
		public void IfAddPropertyIsCalledTwiceWithTheSamePropertyNameShouldThrowException()
		{
			var builder = new ClassBuilder(ClassName);
			const string firstPropertyName = "FirstProperty";

			builder.AddProperty<string>(firstPropertyName).AddProperty<string>(firstPropertyName).GetDeclaration();
		}
	}
}
