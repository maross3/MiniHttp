using System.Diagnostics;
using MiniHttp.Forms;
using MiniHttp.Global;

namespace MiniHttp;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Trace.Listeners.Add(new LogTraceListener("["));
        Application.Run(new InteropForm());
    }
    
}