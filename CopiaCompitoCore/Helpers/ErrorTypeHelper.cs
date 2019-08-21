using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentCore
{
    public static class ErrorTypeHelper
    {
        public static string Message(this AssignmentErrorType errorType)
        {
            switch (errorType)
            {
                case AssignmentErrorType.UnknowError: return "Errore sconosciuto";                    
                case AssignmentErrorType.ProjectTargetNoFound: return "Progetto non trovato";
                case AssignmentErrorType.CopyError: return "Errore di copia";                    
                case AssignmentErrorType.ClearError: return "Errore pulitura cartelle";
                default: return "";
            }
        }
    }
}
