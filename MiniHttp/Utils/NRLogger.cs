using System.Diagnostics;

namespace MiniHttp.Utils;

public static class NrLogger
{
    // todo:
    // add different logging methods
    // add different logging levels
    
    public static void Log(string message)
    {
        Trace.WriteLine(message, $"[{DateTime.Now.TimeOfDay.ToString()}]");
    }
}