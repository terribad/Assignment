using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace AssignmentCore
{
    public class Global
    {
        public const string LinkToExecutable = "CONSEGNA COMPITO";
        public const string SourceDLL = "AssignmentCore.dll";

        public const int RecentSpanTimeForStart = 60;
        public const int RecentSpanTimeForComplete = 30;

        const string DevelopMachineRoot = @"h:\";
        const string VS_PATH = @"Visual Studio 2015\Projects";
        const string VS12_PATH = @"Visual Studio 2012\projects";
        
        const string SD_PATH = @"SharpDevelop Projects";
        const string EASYCPU_PATH = @"EasyCPU Progetti";
        const string PACKETRACER_PATH = VS_PATH;
        const string ACCESS_PATH = VS_PATH;
        const string DevleopAccounts = "paolouser, admin";
        const string AssignmentAccounts = "comp1, comp2";
        
        static readonly string personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        static readonly string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        private static ITimeProvider _time = new DefaultTimeProvider();
        public static ITimeProvider Time
        {
            get {return _time;}
            set { _time = value; }
        }

        public static void Sleep()
        {
            if (IsDevelopAccount)
                Thread.Sleep(100);
            else
                Thread.Sleep(400);
        }

        public static bool IsDevelopAccount
        {
            get { return DevleopAccounts.HasSubstring(Environment.UserName); }
        }

        public static bool IsAssignmentAccount
        {
            get 
            {
                if (IsDevelopAccount)
                    return true;
                return AssignmentAccounts.HasSubstring(Environment.UserName); 
            }
        }

        public static bool IsLocalMachine
        {
            get { return Environment.MachineName.Starts("Robby"); }
        }

        private static string DevelopBasePath
        {
            get { return string.Format(@"{0}__COMPITO", DevelopMachineRoot); }
        }

        public static string Personal
        {
            get {return IsDevelopAccount ? DevelopBasePath+ @"\documenti" : personal;}
        }

        public static string Desktop
        {
            get { return IsDevelopAccount ? DevelopBasePath + @"\desktop" : desktop; }
        }

        public static string TempFolder
        {
            get {return Path.GetTempPath();}
        }

        public static string Projects
        {
            get {return Personal + "\\"+VS_PATH;}
        }

        public static string EasyCpuProjects
        {
            get { return Personal + "\\" + EASYCPU_PATH; }
        }

        public static string SharpDevelopProjects
        {
            get { return Personal + "\\" + SD_PATH; }
        }

        public static string VS12Projects
        {
            get { return Personal + "\\" + VS12_PATH; }
        }

        public static string PacketTracerProjects
        {
            get { return Personal + "\\" + PACKETRACER_PATH; }
        }

        public static string AccessProjects
        {
            get { return Personal + "\\" + ACCESS_PATH; }
        }

        public static string CompleteAssignment
        {
            get { return IsDevelopAccount ? DevelopBasePath + @"\elaborati" : @"\\svrmain\biennio\elaborati"; }                                
        }

        public static string Assignment
        {
            get{ return IsDevelopAccount ? DevelopBasePath + @"\compito" : @"\\svrmain\biennio\compito";}                                
        }

    }
}
