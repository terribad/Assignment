using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AssignmentCore
{
    public class Trash
    {

        [DllImport("Shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlags dwFlags);
        public static void Clean()
        {
             SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlags.SHERB_NOSOUND | RecycleFlags.SHERB_NOCONFIRMATION);
        }
    }
    enum RecycleFlags : int
    {
        // Nessuna finestra di conferma
        SHERB_NOCONFIRMATION = 0x00000001,
        // Nessuna finestra di progresso
        SHERB_NOPROGRESSUI = 0x00000001,
        // Nessun suono
        SHERB_NOSOUND = 0x00000004
    }
}
