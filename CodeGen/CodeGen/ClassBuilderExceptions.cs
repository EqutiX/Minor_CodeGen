using System;

namespace CodeGen
{
    /// <summary>
    /// Exception that is thrown when a Field is not found.
    /// </summary>
	public class FieldNotFoundException : Exception
	{

	}

    /// <summary>
    /// Exception that is thrown when a Method already exists.
    /// </summary>
	public class MethodAlreadyExistsException : Exception
	{

	}

    /// <summary>
    /// Exception that is thrown when a Field already exists.
    /// </summary>
	public class FieldAlreadyExistsException : Exception
	{

	}


    /// <summary>
    /// Exception that is thrown when a Property is not found.
    /// </summary>
	public class PropertyNotFoundException : Exception
	{

	}


    /// <summary>
    /// Exception that is throw when a Property already exists.
    /// </summary>
	public class PropertyAlreadyExistsException : Exception
	{

	}
}
