using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestClasses
{

    public static class LogExtensions
    {

        public static void Log(this Object This, string Txt, [CallerMemberName] string Member = null)
        {
            Debug.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]{This.ToString()}].({Member}) '{Txt}' ");
        }

        public static void Log(this Object This, Exception E, [CallerMemberName] string Member = null)
        {
            Debug.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]{This.ToString()}].({Member}) Exception='{E}' ");
        }

    }

}
