using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SynergyTemplate
{
    public class SafeInvoke
    {
        MethodInfo method;
        public SafeInvoke(MethodInfo _Method)
        {
            method = _Method;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Instance">owner of the method ( null for constructors static classes and other static stuff )</param>
        /// <param name="_Parameters">parameters</param>
        /// <returns>true if success, false if failed</returns>
        public bool Invoke(object _Instance, object[] _Parameters)
        {
            object result = null;
            try
            {
                result = method.Invoke(_Instance, _Parameters);
                return true;
            }
            catch (TargetInvocationException e)
            {
                //System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e.InnerException, true);
                Log.Write("SafeInvoke", Log.Line.Type.Error, @"Invocation error : Method {0}", method.Name);
                Log.Write("SafeInvoke", Log.Line.Type.Error, @"Invocation error : {0}", e.InnerException.ToString());
                //Log.Write("SafeInvoke", Log.Line.Type.Error, @"Stacktrace : {0}", e.InnerException.StackTrace);
            }
            catch (TargetParameterCountException)
            {
                Log.Write("SafeInvoke", Log.Line.Type.Error, "Method does not have {0} parameters", _Parameters.Length);
            }
            return false;
        }
    }
}
