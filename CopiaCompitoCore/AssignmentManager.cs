using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssignmentCore
{
    public class AssignmentManager
    {

        public static readonly IReadOnlyList<string> SkipProcessToClose = new string[]
        {
            "shellexperiencehost"
        };
        public static AssignmentStart CreateStart(Config config = null)
        {
            if (config == null)
                config = new Config();
            return new AssignmentStart(config);
        }
    }

    public enum AssignmentErrorType
    {
        UnknowError,
        ProjectTargetNoFound,
        CopyError,
        ClearError
    }

    public class AssignmentErrorEventArgs : EventArgs
    {
        public static AssignmentErrorEventArgs New(AssignmentErrorType errorType, Exception error = null)
        {
            return new AssignmentErrorEventArgs() { ErrorType = errorType, Error = error };
        }
        public AssignmentErrorType ErrorType;
        public Exception Error;
    }
}
