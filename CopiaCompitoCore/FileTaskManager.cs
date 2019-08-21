using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using IWshRuntimeLibrary;

namespace AssignmentCore
{
    public class FileTaskManager
    {
        public static bool IsSharedPath(string path, bool forceAsShared = false)
        {
            if (forceAsShared)
                return true;
            return Path.GetPathRoot(path).StartsWith("\\");
        }

        readonly Config config;
        public FileTaskManager(Config config)
        {
            this.config = config;
        }

        public string[] GetProjectNames()
        {
            if (config.ProjectTarget.TargetType == TargetType.Folder)
                return Directory.GetDirectories(config.SourcePath, config.ProjectName);
            return Directory.GetFiles(config.SourcePath, config.ProjectName);
        }

        public void DeleteFile(string path)
        {
            if (FileExists(path))
                System.IO.File.Delete(path);
        }

        public bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public bool TargetExists(string path)
        {
            return System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
        }

        public bool FolderExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
                System.IO.Directory.Delete(path, true);
        }

        public void DeleteFolder(string path, bool fileOnly = false)
        {
            if (!Directory.Exists(path))
                return;

            if (!fileOnly)
            {
                System.IO.Directory.Delete(path, true);
                return;
            }

            var list = Directory.GetFiles(path);
            foreach (var f in list)
            {
                System.IO.File.Delete(f);
                Debug.WriteLine("Eliminato {0}", f);
            }

            list = Directory.GetDirectories(path);
            foreach (var f in list)
            {
                DeleteFolder(f, false);
            }
        }


        public void CreateLink(string linkName, string targetPath, string args = "")
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + "\\" + linkName + ".lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Arguments = args;
            shortcut.Description = "Link per consegna compito";
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            //shortcut.Hotkey = "Ctrl+Shift+N";
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        public void ClearFolders(VirtualPath[] folders)
        {
            foreach (var folder in folders)
            {
                ClearFolder(folder);
            }
        }

        private void ClearFolder(VirtualPath folder, string skipExtensions = ".lnk")
        {
            string[] list = Directory.GetDirectories(folder.Path);
            foreach (var f in list)
            {
                Directory.Delete(f, true);
                Debug.WriteLine("Eliminata {0}", f);
            }

            list = Directory.GetFiles(folder.Path);
            foreach (var f in list)
            {
                if (!skipExtensions.HasSubstring(Path.GetExtension(f)))
                {
                    System.IO.File.Delete(f);
                    Debug.WriteLine("Eliminato {0}", f);
                }
            }
        }

        public DateTime GetWriteTime(string path)
        {
            return new FileInfo(path).LastWriteTime;
        }

        public void SetWriteTime(string path)
        {
            new FileInfo(path).LastWriteTime = Global.Time.Now();
        }

        public bool IsRecent(int minutes, string path)
        {
            DateTime time = GetWriteTime(path);
            var min = (Global.Time.Now() - time).TotalMinutes;
            return min < minutes;
        }

        public void CopyFile(string source, string dest, bool overwrite = true)
        {
            if (!MatchFilter(Path.GetFileName(source)))
                System.IO.File.Copy(source, dest, overwrite);
        }



        readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
        private bool MatchFilter(string name)
        {
            return config.Filter.Contains(name, comparer);
        }

        public void CopyFolderTo(string source, string dest, string nameSourceAs = null, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo sourceDir = new DirectoryInfo(source);

            if (!sourceDir.Exists)
                throw new DirectoryNotFoundException("La cartella non esiste o non è stata trovata: " + source);


            dest = Path.Combine(dest, nameSourceAs ?? sourceDir.Name);

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            DirectoryInfo[] dirs = sourceDir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = sourceDir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (MatchFilter(file.Name))
                    continue;
                string temppath = Path.Combine(dest, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo dir in dirs)
                {
                    if (MatchFilter(dir.Name))
                        continue;
                    CopyFolderTo(dir.FullName, dest, dir.Name, copySubDirs);
                }
            }
        }
    }
}
