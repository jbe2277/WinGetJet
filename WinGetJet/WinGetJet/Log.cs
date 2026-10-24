using NLog;

namespace WinGetJet;

internal static class Log
{
    public static Logger Default { get; } = LogManager.GetLogger("WGJ");
}