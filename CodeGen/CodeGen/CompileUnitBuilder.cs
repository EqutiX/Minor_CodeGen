using System.CodeDom;
using System.Diagnostics;

namespace CodeGen
{
    public class CompileUnitBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly CodeCompileUnit _currentCodeCompileUnit;

        private readonly CodeNamespace _codeNamespace;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceName"></param>
        public CompileUnitBuilder(string namespaceName)
        {
            if(string.IsNullOrWhiteSpace(namespaceName)) throw new NamespaceNameCanNotBeEmptyOrNullException();
            _codeNamespace = new CodeNamespace(namespaceName);

            _currentCodeCompileUnit = new CodeCompileUnit();
            _currentCodeCompileUnit.Namespaces.Add(_codeNamespace);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CodeCompileUnit GetCompileUnit()
        {
            return _currentCodeCompileUnit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        public CompileUnitBuilder AddClass(CodeTypeDeclaration classDeclaration)
        {
            if(classDeclaration == null) throw new ClassDeclarationCanNotBeNullException();
            _codeNamespace.Types.Add(classDeclaration);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usingName"></param>
        /// <returns></returns>
        public CompileUnitBuilder AddUsing(string usingName)
        {
            if(string.IsNullOrWhiteSpace(usingName)) throw new UsingCanNotBeNullOrEmptyException();
            _codeNamespace.Imports.Add(new CodeNamespaceImport(usingName));
            return this;
        }
    }
}
