using System.CodeDom;
using CodeGen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class CompileUnitBuilderTests
    {
        private const string NamespaceName = "TestSpace";

        [TestMethod]
        public void IfNewIntanceOfCompileUnitBuilderShouldHaveNamespaceWithGivenNamespaceName()
        {
            var builder = new CompileUnitBuilder(NamespaceName);

            var target = builder.GetCompileUnit();

            Assert.AreEqual(target.Namespaces.Count, 1);
            Assert.AreEqual(target.Namespaces[0].Name, NamespaceName);
        }

        [TestMethod]
        [ExpectedException(typeof (NamespaceNameCanNotBeEmptyOrNullException))]
        public void IfNewIntanceOfCompileUnitBuilderWithNullAsNamespaceNameShouldThrowException()
        {
            var builder = new CompileUnitBuilder(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NamespaceNameCanNotBeEmptyOrNullException))]
        public void IfNewIntanceOfCompileUnitBuilderWithAnEmptyStringAsNamespaceNameShouldThrowException()
        {
            var builder = new CompileUnitBuilder(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(NamespaceNameCanNotBeEmptyOrNullException))]
        public void IfNewIntanceOfCompileUnitBuilderWithAWhitspaceStringAsNamespaceNameShouldThrowException()
        {
            var builder = new CompileUnitBuilder(" ");
        }

        [TestMethod]
        public void IsNewTypeIsAddedNamespaceHaveTypeWithGivenType()
        {
            const string typeName = "testType";
            var builder = new CompileUnitBuilder(NamespaceName);
            var classDeclaration = new CodeTypeDeclaration(typeName);
            var target = builder.AddClass(classDeclaration).GetCompileUnit();

            Assert.AreEqual(1,target.Namespaces[0].Types.Count);

            Assert.AreEqual(typeName, target.Namespaces[0].Types[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof (ClassDeclarationCanNotBeNullException))]
        public void IfClassDeclarationIsNullShouldThrowException()
        {
            var builder = new CompileUnitBuilder(NamespaceName);
            builder.AddClass(null);
        }

        [TestMethod]
        public void IfUsingIsAddedNamespaceShouldHaveGivenUsing()
        {
            const string usingName = "system";
            var builder = new CompileUnitBuilder(NamespaceName);

            var target = builder.AddUsing(usingName).GetCompileUnit();

            Assert.AreEqual(target.Namespaces[0].Imports.Count, 1);
        }


        [TestMethod]
        [ExpectedException(typeof(UsingCanNotBeNullOrEmptyException))]
        public void IfUsingIsAddedWithNullAsUsingShouldThowException()
        {
            var builder = new CompileUnitBuilder(NamespaceName);

            builder.AddUsing(null);
        }

        [TestMethod]
        [ExpectedException(typeof(UsingCanNotBeNullOrEmptyException))]
        public void IfUsingIsAddedWithAnEmptyAsUsingShouldThowException()
        {
            var builder = new CompileUnitBuilder(NamespaceName);

            builder.AddUsing("");
        }

        [TestMethod]
        [ExpectedException(typeof(UsingCanNotBeNullOrEmptyException))]
        public void IfUsingIsAddedWithAWhitespacedStringAsUsingShouldThowException()
        {
            var builder = new CompileUnitBuilder(NamespaceName);

            builder.AddUsing(" ");
        }

    }
}
