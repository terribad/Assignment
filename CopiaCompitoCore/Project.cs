using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssignmentCore
{
    public class Project
    {
        public string SourceFullName
        {
            get { return System.IO.Path.Combine(SourcePath, Name); }
        }

        public string DestFullName
        {
            get { return System.IO.Path.Combine(SourcePath, NameAs); }
        }

        private VirtualPath _sourcePath;
        public VirtualPath SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (!Directory.Exists(value))
                    throw new ArgumentException("Il percorso sorgente del progetto non esiste!", "SourcePath");
                _sourcePath = value;
            }
        }

        private VirtualPath _destPath;
        public VirtualPath DestPath
        {
            get { return _destPath; }
            set
            {
                if (!Directory.Exists(value))
                    throw new ArgumentException("Il percorso destinazione del progetto non esiste!", "DestPath");
                _destPath = value;
            }
        }
        public string Name { get; set; }
        public string NameAs { get; set; }
        public string Target { get; set; }
        public string TargetDescription { get; set; }
        public ProjectType Type
        {
            get { return string.IsNullOrEmpty(Ext) ? ProjectType.Folder : ProjectType.File; }
        }
        public string Ext { get { return System.IO.Path.GetExtension(Name); } }
        
    }

    public enum ProjectType
    {
        File,
        Folder
    }
    
}
