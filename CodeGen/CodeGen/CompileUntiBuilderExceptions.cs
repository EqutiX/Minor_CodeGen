using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    /// <summary>
    /// This Exception is thrown if the given namescape is empty or null.
    /// </summary>
    public class NamespaceNameCanNotBeEmptyOrNullException: Exception
    {
    }


    /// <summary>
    /// This Exception is thrown if the given Class name is empty or null.
    /// </summary>
    public class ClassDeclarationCanNotBeNullException : Exception
    {
        
    }

    /// <summary>
    /// This Exception is thrown if the Using given is empty or null.
    /// </summary>
    public class UsingCanNotBeNullOrEmptyException : Exception
    {
        
    }

    /// <summary>
    /// This Exception is thrown if the Interface name given is empty or null.
    /// </summary>
	public class InterfaceDeclarationCanNotBeNullException : Exception
	{

	}
}
