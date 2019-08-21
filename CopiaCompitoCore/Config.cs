using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssignmentCore
{
    public class Config
    {
        const string DEFAULT_FILENAME = "Config.ini";
        const string PROJECT_NAME = "projectName";
        const string SOURCE_PATH = "sourcePath";
        const string DEST_PATH = "destPath";
        const string ASSIGNMENT_PATH = "assignmentPath";
        const string CLEAR_FOLDERS = "clearFolders";                
        const string CHECK_DOUBLE_START= "checkDoubleStart";
        const string ASSIGNMENT_MODE = "assignmentMode";
        const string FILTER = "filter";
        const string OPEN = "open";

        readonly Dictionary<string, ProjectTargetInfo> targets;
        

        readonly string fileName;
        public Config(string fileName = DEFAULT_FILENAME)
        {
            if (!File.Exists(fileName))
            {
                string msg = string.Format("{0}\n[{1}]", "Il file di configurazione non è stato trovato!", fileName);
                throw new InvalidOperationException(msg);
            }
            this.fileName = fileName;

            targets = new Dictionary<string, ProjectTargetInfo>(StringComparer.CurrentCultureIgnoreCase);            
            SetDefaults();
            LoadConfig(fileName);
        }

        public bool IsDefaultConfig { get; private set; }
        private void LoadConfig(string fileName)
        {
            var lines = File.ReadAllLines(fileName).Where(l => !l.IsEmpty() && !l.StartsWith("#"));
            ParseConfig(lines);
        }

        public Exception LoadConfigError { get; private set; }
        public AssignmentMode AssignmentMode { get; private set; }
        public string ProjectName { get; private set; }             
        public VirtualPath SourcePath { get; private set; }
        public VirtualPath DestPath { get; private set; }
        public VirtualPath AssignmentPath { get; private set; }        
        public VirtualPath ProjectFullName
        {
            get { return Path.Combine(SourcePath, ProjectName); }
        }
        public ProjectTargetInfo ProjectTarget { get; private set; }    
        public VirtualPath[] ClearFolders { get; private set; }
        public bool CheckDoubleAssignmentStart { get; private set; }
        public string[] Filter { get; private set; }

        private List<VirtualPath> _openList;
        public IReadOnlyList<VirtualPath> OpenList
        {
            get { return _openList; }
        }

        private string _tempExecutablePath;
        public string TempExecutablePath
        {
            get 
            { 
                if (_tempExecutablePath == null)
                {
                    var exeName = ProcessManager.ExecutableName;
                    _tempExecutablePath = Path.Combine(Global.TempFolder, exeName);
                }
                return _tempExecutablePath;
            }
        }
        public void SaveForCompleteAssignment(string projectName, string path)
        {
            path = Path.Combine(path, fileName);
            using (var sw = File.CreateText(path))
            {
                sw.WriteLine("#File di configurazione CONSEGNA_COMPITO");
                sw.WriteLine("{0} = {1}", ASSIGNMENT_MODE, AssignmentMode.Complete);
                
                sw.WriteLine("{0} = {1}|{2}", PROJECT_NAME, projectName+ProjectTarget.Ext, ProjectTarget.Key);

                sw.WriteLine("{0} = {1}", SOURCE_PATH, DestPath);
                sw.WriteLine("{0} = {1}", DEST_PATH, AssignmentPath);

                var paths = string.Join("|", ClearFolders.Select(vp=>vp.RawPath));
                sw.WriteLine("{0} = {1}", CLEAR_FOLDERS, paths);
                var filter = string.Join("|", Filter);
                sw.WriteLine("{0} = {1}", FILTER, filter);
            }
        }
        private void SetDefaults()
        {
            targets["acs"] = new ProjectTargetInfo("MsAccess", "Access", "", "acs");
            targets["odt"] = new ProjectTargetInfo("soffice", "Writer", ".odt");
            targets["ods"] = new ProjectTargetInfo("soffice", "Calc", ".ods");
            targets["sln"] = new ProjectTargetInfo("devenv", "Visual Studio", "", "sln");
            targets["sd"] = new ProjectTargetInfo("sharpdevelop", "Sharp Develop", "", "sd");
            targets["as"] = new ProjectTargetInfo("EasyCPU", "Easy CPU", "", "as");
            targets["pt"] = new ProjectTargetInfo("PacketTracer6", "Packet Tracer", "", "pt");
            ProjectTarget = targets["sln"];
            
            AssignmentMode = AssignmentMode.Start;
            ClearFolders = new VirtualPath[0];
            CheckDoubleAssignmentStart = true;
            Filter = new string[2] {"bin", "obj"};
            _openList = new List<VirtualPath>();            
            IsDefaultConfig = true;
        }

        private void ParseConfig(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                string[] fields = line.SplitTrim("=");
                if (fields.Length < 2)
                    ConfigException.Raise("{0}: valore mancante", fields[0]);
                ParseConfigItem(fields[0], fields[1]);
            }
            if (ProjectName == null)
                ConfigException.Raise("Nome progetto non è impostato", "ProjectName");
            if (SourcePath == null)
                ConfigException.Raise("Percorso sorgente non è impostato", "SourcePath");
            if (DestPath == null)
                ConfigException.Raise("Percorso destinazione non è impostato", "DestPath");
            if (AssignmentPath == null && AssignmentMode == AssignmentMode.Start)
                ConfigException.Raise("Percorso compito non è impostato", "AssignmentPath");
            IsDefaultConfig = false;
        }

        private void ParseConfigItem(string name, string value)
        {
            if (name.Eq(ASSIGNMENT_MODE))
            {
                AssignmentMode = (AssignmentMode)Enum.Parse(typeof(AssignmentMode),  value, true);
            }
            else if (name.Eq(PROJECT_NAME))
            {
                string[] items = value.SplitTrim("|");
                ProjectName = items[0];
                if (items.Length > 1)
                    SetTarget(items[1]);
                else
                {
                    var ext = Path.GetExtension(ProjectName);
                    if (ext != "")
                        SetTarget(ext.Substring(1));
                }
            }
            else if (name.Eq(DEST_PATH))
            {
                DestPath = value;
            }
            else if(name.Eq(SOURCE_PATH))
            {
                SourcePath = value;
            }
            else if (name.Eq(ASSIGNMENT_PATH))
            {
                AssignmentPath = value;
            }
            else if (name.Eq(CLEAR_FOLDERS))
            {
                ClearFolders = ParseList(value).Select(i => new VirtualPath(i)).ToArray();
            }            
            else if (name.Eq(CHECK_DOUBLE_START))
            {
                CheckDoubleAssignmentStart = bool.Parse(value);
            }
            else if (name.Eq(FILTER))
            {
                Filter = ParseList(value).ToArray();
            }
            else if (name.Eq(OPEN))
            {
                var path = Path.Combine(SourcePath, value);
                _openList.Add(path);
            }
        }

        private void SetTarget(string value)
        {
            if (targets.ContainsKey(value))
                ProjectTarget = targets[value];
        }

        private IEnumerable<string> ParseList(string folders)
        {
            return folders.SplitTrim("|");
        }
    }

    //public class ProjectInfo
    //{
    //    public string ProjectPattern { get; private set; }
    //    public string ProjectName { get; private set; }        
    //    public VirtualPath ProjectFolder { get; private set; }
    //    public VirtualPath ProjectFullName
    //    {
    //        get { return Path.Combine(ProjectFolder, ProjectName); }
    //    }
    //}

    public class ProjectTargetInfo
    {
        public ProjectTargetInfo(string target, string desc, string ext, string key = "")
        {
            this.Target = target;
            this.Description = desc;
            this.Ext = ext;
            this._key = key;
        }
        public string Target { get; private set; }
        public string Description { get; private set; }
        public TargetType TargetType
        {
            get { return string.IsNullOrEmpty(Ext) ? TargetType.Folder : TargetType.File; }
        }
        public string Ext { get; private set; }

        private string _key = "";
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                    return Ext;
                return _key;
            }
        }
    }

    public enum TargetType
    {
        File,
        Folder
    }

    public enum AssignmentMode
    {
        Start,
        Complete
    }

    public class ConfigException:ApplicationException
    {
        public ConfigException(string message) : base(message) { }
        public ConfigException(string message, Exception inner) : base(message, inner) { }

        public static void Raise(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            throw new ConfigException(msg);
        }
    }
}
