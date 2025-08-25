using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Simplifies working with different scenes
    /// </summary>
    public abstract class Script
    {
        private readonly bool _canUpdate;
        private readonly bool _canRender;

        public Script()
        {
            _canUpdate = FunctionInvoker.CheckFunctionExists("Update", this);
            _canRender = FunctionInvoker.CheckFunctionExists("Render", this);
        }

        #region Static

        /// <summary>
        /// Gets and sets the currently active script
        /// </summary>
        public static Script Active
        {
            get => _active;
            set 
            { 
                if (value != null)
                {
                    FunctionInvoker.Invoke("Replaced", _active);
                    _active = value;
                    FunctionInvoker.Invoke("Start", _active);
                }
            }
        }

        private static Script _active;
        private static Type _type;

        /// <summary>
        /// Used to assign what script to start on startup
        /// </summary>
        public static void AssignStartupScript<T>() where T : Script
        {
            _type = typeof(T);
        }

        /// <summary>
        /// Starts the assigned script
        /// </summary>
        internal static void StartScript()
        { 
            if (_type == null) throw new NullReferenceException("No script is assigned");
            Active = Activator.CreateInstance(_type, false) as Script;
        }

        /// <summary>
        /// Invoke the Update function in the script
        /// </summary>
        internal static void UpdateScript()
        {
            if(_active._canUpdate) FunctionInvoker.Invoke("Update", _active);
        }

        /// <summary>
        /// Invoke the Render function in the script
        /// </summary>
        internal static void RenderScript()
        {
            if (_active._canRender) FunctionInvoker.Invoke("Render", _active);
        }

        /// <summary>
        /// Invoke the Resize function in the script
        /// </summary>
        internal static void ResizeScript()
        {
            FunctionInvoker.Invoke("Resize", _active);
        }

        #endregion
    }
}