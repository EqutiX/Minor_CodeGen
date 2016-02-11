using System;

namespace CodeGen
{
    /// <summary>
    /// ParameterItem is a helper class to simple pass parameters to Method create functions.
    /// </summary>
    public class ParameterItem
    {
        /// <summary>
        /// Type of the parameter.
        /// </summary>
        public Type Type { get; set; }
        
        /// <summary>
        /// Name of the parameter.
        /// </summary>
        public string Name { get; set; }   
    }
}