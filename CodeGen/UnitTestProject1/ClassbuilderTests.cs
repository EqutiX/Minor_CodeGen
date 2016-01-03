using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeGen;

namespace UnitTestProject1
{
	[TestClass]
	public class ClassBuilderTests
	{
        const string ClassName = "FirstClass";
		const string FunctionName = "FirstFunction";
		const string FirstFieldName = "FirstField";
		const string FirstFieldValue = "FirstValue";
		const string FirstPropertyName = "FirstProperty";

		[TestMethod]
		public void IfNewIntanceOfClassBuilderGetDeclarationShouldReturnClassWithGivenName()
		{
			
			var builder = new ClassBuilder( ClassName );

			var target = builder.GetDeclaration();

			Assert.AreEqual( target.Name, ClassName );
		}

		[TestMethod]
		public void IfAddFieldIsCalledShouldAddFieldToDeclaration()
		{
			
			var builder = new ClassBuilder( ClassName );

			var target = builder.AddField<string>( FirstFieldName ).GetDeclaration();

			Assert.AreEqual( 1, target.Members.Count );
			var member = target.Members[0];
			Assert.AreEqual( FirstFieldName, member.Name );
		}

        [TestMethod]
        [ExpectedException(typeof(FieldAlreadyExistsException))]
        public void IfAddFieldIsCalledTwiceWithTheSameFieldNameShouldThrowException()
        {

            var builder = new ClassBuilder(ClassName);
            

            builder.AddField<string>(FirstFieldName).AddField<string>(FirstFieldName).GetDeclaration();

        }

        [TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void IfAddFieldValueToNonExistingFieldShouldThrowException()
		{

			var builder = new ClassBuilder( ClassName );
			

			builder.AddFieldValue( FirstFieldName, FirstFieldValue ).GetDeclaration();
		}

		[TestMethod]
		public void IfAddFieldValueToExistingFieldShouldNotThrowException()
		{

			var builder = new ClassBuilder( ClassName );

			var target = builder.AddField<string>( FirstFieldName ).AddFieldValue( FirstFieldName, FirstFieldValue ).GetDeclaration();
			Assert.IsNotNull( target );
		}

		[TestMethod]
		public void IfAddMethodIsCalledShouldAddNewFunctionWithGivenName()
		{

			var builder = new ClassBuilder( ClassName );

			var target = builder.AddMethod<string>(FunctionName, new ParameterItem[]{}).GetDeclaration();

            Assert.AreEqual(1, target.Members.Count);
            var member = target.Members[0];
            Assert.AreEqual(FunctionName, member.Name);
        }

	    [TestMethod]
	    [ExpectedException(typeof(MethodAlreadyExistsException))]
	    public void IfAddMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
	    {

            var builder = new ClassBuilder(ClassName);

            builder.AddMethod<string>(FunctionName, new ParameterItem[] { }).AddMethod<string>(FunctionName, new ParameterItem[] { }).GetDeclaration();
        }

	    [TestMethod]
	    public void IfAddVoidMethodIsCalledANewMethodShouldBeAddedToTheDeclarationMembers()
	    {

            var builder = new ClassBuilder(ClassName);

	        var target = builder.AddVoidMethod(FunctionName, new ParameterItem[] { }).GetDeclaration();

            Assert.AreEqual(1,target.Members.Count);
            Assert.AreEqual(FunctionName, target.Members[0].Name);
	    }

        [TestMethod]
        [ExpectedException(typeof(MethodAlreadyExistsException))]
        public void IfAddVoidMethodIsCalledTwiceWithTheSameNameAnExceptionShouldBeenThrown()
        {
            var builder = new ClassBuilder(ClassName);
            
					
            builder.AddVoidMethod(FunctionName, new ParameterItem[] { }).AddVoidMethod(FunctionName, new ParameterItem[] { }).GetDeclaration();
        }

		[TestMethod]
		public void IfAddPropertyIsCalledShouldAddPropertyToDeclaration()
		{
			var builder = new ClassBuilder(ClassName);
			

			var target = builder.AddProperty<string>(FirstPropertyName).GetDeclaration();

			Assert.AreEqual(1, target.Members.Count);
			var member = target.Members[0];
			Assert.AreEqual(FirstPropertyName, member.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(PropertyAlreadyExistsException))]
		public void IfAddPropertyIsCalledTwiceWithTheSamePropertyNameShouldThrowException()
		{
			var builder = new ClassBuilder(ClassName);

			builder.AddProperty<string>(FirstPropertyName).AddProperty<string>(FirstPropertyName).GetDeclaration();
		}

		[TestMethod]
		public void IfAddEntryPointIsCalledShouldAddEntryPointToDeclaration()
		{
			var builder = new ClassBuilder(ClassName);
			var target = builder.AddEntryPoint<int>(FunctionName).GetDeclaration();

			var member = target.Members[0];
			Assert.AreEqual(FunctionName, member.Name);
		}
	}
}
