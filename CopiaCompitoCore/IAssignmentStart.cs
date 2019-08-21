using System;

namespace AssignmentCore
{
    public interface IAssignmentStart
    {
        event EventHandler<CheckDoubleStartEventArgs> AssignmentCheckDoubleStart;
        event EventHandler<ProjectNameRequestEventArgs> AssignmentRequestProjectName;
        event EventHandler AssignmentStartAborted;
        event EventHandler AssignmentStartCompleted;
        event EventHandler<AssignmentErrorEventArgs> AssignmentStartError;

        void CopyProject(string sourceName, string nameAs);
        void Execute();
    }
}