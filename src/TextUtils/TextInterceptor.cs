using System.Diagnostics;
using Castle.DynamicProxy;

namespace TextUtils
{
    public class TextInterceptor : IInterceptor
    {
        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            Debug.WriteLine(invocation.InvocationTarget.ToString() + ": " + (string?) invocation.Method.Name);

            invocation.Proceed();
            if (invocation.ReturnValue == null)
            {
                Debug.WriteLine("Return value is null");
            }
            else
            {
                Debug.WriteLine("Return value is " + invocation.ReturnValue);

            }
        }
    }
}