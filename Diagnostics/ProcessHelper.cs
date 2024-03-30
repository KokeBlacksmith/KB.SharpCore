using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KB.SharpCore.Diagnostics;

public class ProcessHelper
{
    // TODO: Allow different OS, now it is only Windows, but MacOs and Linux should be supported
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);


    public static bool BringProcessToFront(Process process)
    {
        if (process == null)
        {
            return false;
        }

        IntPtr hWnd = process.MainWindowHandle;
        if (hWnd == IntPtr.Zero)
        {
            return false;
        }

        return SetForegroundWindow(hWnd);
    }

    public static bool IsProcessAlive(Process process)
    {
        if (process == null)
        {
            return false;
        }

        try
        {
            Process.GetProcessById(process.Id);
            return true; // Process is still alive
        }
        catch (ArgumentException)
        {
            return false; // Process has exited
        }
    }
}
