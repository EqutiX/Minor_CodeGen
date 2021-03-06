﻿using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace CodeGen
{
    /// <summary>
    /// CompileUnitBuilder is class to Create a unit that can be compiled.
    /// </summary>
    public class CompileUnitBuilder
    {
        /// <summary>
        /// Store a Single instance of the CodeCompileUnit
        /// </summary>
        private readonly CodeCompileUnit _currentCodeCompileUnit;

		/// <summary>
		/// Store a Single instance of the CodeNamespace
		/// </summary>
		private readonly CodeNamespace _codeNamespace;

        /// <summary>
        /// Default constructor to create the namespace
        /// </summary>
        /// <param name="namespaceName">Name of the namespace</param>
        public CompileUnitBuilder(string namespaceName)
        {
            if(string.IsNullOrWhiteSpace(namespaceName)) throw new NamespaceNameCanNotBeEmptyOrNullException();
            _codeNamespace = new CodeNamespace(namespaceName);

            _currentCodeCompileUnit = new CodeCompileUnit();
            _currentCodeCompileUnit.Namespaces.Add(_codeNamespace);

        }

        /// <summary>
        /// Get the single CodeCompileUnit
        /// </summary>
        /// <returns>The single CodeCompileUnit instance</returns>
        public CodeCompileUnit GetCompileUnit()
        {
            return _currentCodeCompileUnit;
        }

		/// <summary>
		/// Adds a class to the current namespace
		/// </summary>
		/// <param name="classDeclaration">Type declaration of the class</param>
		/// <returns>The single CompileUnitBuilder instance</returns>
		public CompileUnitBuilder AddClass(CodeTypeDeclaration classDeclaration)
        {
            if(classDeclaration == null) throw new ClassDeclarationCanNotBeNullException();
            _codeNamespace.Types.Add(classDeclaration);
            return this;
        }

		/// <summary>
		/// Adds an interface to the current namespace
		/// </summary>
		/// <param name="interfaceDeclaration">Type declaration of the interface</param>
		/// <returns>The single CompileUnitBuilder instance</returns>
		public CompileUnitBuilder AddInterface(CodeTypeDeclaration interfaceDeclaration)
		{
			if (interfaceDeclaration == null) throw new InterfaceDeclarationCanNotBeNullException();
			_codeNamespace.Types.Add(interfaceDeclaration);
			return this;
		}

		/// <summary>
		/// Adds an import to the current namespace.
		/// </summary>
		/// <param name="usingName">Name of the import</param>
		/// <returns>The single CompileUnitBuilder instance</returns>
		public CompileUnitBuilder AddUsing(string usingName)
        {
            if(string.IsNullOrWhiteSpace(usingName)) throw new UsingCanNotBeNullOrEmptyException();
            _codeNamespace.Imports.Add(new CodeNamespaceImport(usingName));
            return this;
        }

        /// <summary>
        /// Creates a sourcecode file containing all code generated
        /// </summary>
        /// <param name="fileName">Name of the sourcefiles</param>
        public void PublishCode(string fileName)
        {
		    StreamWriter sourceWriter;

		    var provider = CodeDomProvider.CreateProvider("CSharp");
		    var provider2 = CodeDomProvider.CreateProvider("CPP");
			var provider3 = CodeDomProvider.CreateProvider("VB");
			var provider4 = CodeDomProvider.CreateProvider("JScript");
			var options = new CodeGeneratorOptions {BracingStyle = "C"};
            using (sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    _currentCodeCompileUnit, sourceWriter, options);
            }
			using (sourceWriter = new StreamWriter(fileName.Replace("cs", "cpp")))
			{
				provider2.GenerateCodeFromCompileUnit(
					_currentCodeCompileUnit, sourceWriter, options);
			}
			using (sourceWriter = new StreamWriter(fileName.Replace("cs", "vb")))
			{
				provider3.GenerateCodeFromCompileUnit(
					_currentCodeCompileUnit, sourceWriter, options);
			}
			using (sourceWriter = new StreamWriter(fileName.Replace("cs", "js")))
			{
				provider4.GenerateCodeFromCompileUnit(
					_currentCodeCompileUnit, sourceWriter, options);
			}
		}
    }
}
