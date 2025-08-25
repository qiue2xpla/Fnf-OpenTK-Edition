using System.Reflection;

namespace Fnf.Framework
{
    /// <summary>
    /// Used to call public and private functions in a class
    /// </summary>
    internal static class FunctionInvoker
    {
        /// <summary>
        /// Checks if a function exists in the given object
        /// </summary>
        public static bool CheckFunctionExists(string FunctionName, object ClassObject)
        {
            return ClassObject.GetType().GetMethod(FunctionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null;
        }

        /// <summary>
        /// Invokes a function in the given class with the given name 
        /// </summary>
        public static void Invoke(string FunctionName, object ClassObject)
        {
            Invoke(FunctionName, ClassObject, null);
        }

        /// <summary>
        /// Invokes a function in the given class with the given name 
        /// </summary>
        public static void Invoke(string FunctionName, object ClassObject, params object[] parameters)
        {
            if (ClassObject == null) return;
            ClassObject.GetType().GetMethod(FunctionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(ClassObject, parameters);
        }
    }
}