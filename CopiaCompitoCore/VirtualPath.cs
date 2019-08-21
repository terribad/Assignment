using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AssignmentCore
{
    public struct VirtualPath
    {
        static Dictionary<string, Func<string, string>> templates = new Dictionary<string, Func<string, string>>();
        public static void SetDefaultTemplates()
        {
            templates = new Dictionary<string, Func<string, string>>();
            templates["$desktop"] = (s => Global.Desktop);
            templates["$document"] = (s => Global.Personal);
            templates["$projects"] = (s => Global.Projects);
            templates["$easyprojects"] = (s => Global.EasyCpuProjects);
            templates["$sharprojects"] = (s => Global.SharpDevelopProjects);
            templates["$vs12projects"] = (s => Global.VS12Projects);
            templates["$ptprojects"] = (s => Global.PacketTracerProjects);
            templates["$accessprojects"] = (s => Global.AccessProjects);
            templates["$temp"] = (s => Global.TempFolder);
        }

        public static void Set(string key, Func<string, string> fun)
        {
            templates[key] = fun;
        }
    
        
        public VirtualPath(string path)
        {
            cachePath = null;
            _rawPath = path;
        }

        private string _rawPath;
        public string RawPath
        {
            get { return _rawPath; }
            private set { _rawPath = value; }
        }

        string cachePath;
        public string Path
        {
            get 
            {
                if (cachePath != null)
                    return cachePath;

                cachePath = RawPath;
                foreach (var kv in templates)
                {
                    if (cachePath.HasSubstring(kv.Key))
                    {
                        var path = kv.Value(kv.Key);
                        cachePath = cachePath.Replace(kv.Key, path);
                        break;
                    }
                }
                cachePath = System.IO.Path.GetFullPath(cachePath);
                return cachePath;
            }
        }

        public static bool operator ==(VirtualPath str1, VirtualPath str2)
        {
            return string.Equals(str1.RawPath, str2.RawPath, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool operator !=(VirtualPath str1, VirtualPath str2)
        {
            return !(str1 == str2);
        }

        public static implicit operator VirtualPath(string s)
        {
            return new VirtualPath(s);
        }

        public static implicit operator string(VirtualPath str)
        {
            return str.Path;
        }

        public override bool Equals(object obj)
        {
            return RawPath.Equals(obj);
        }

        public override int GetHashCode()
        {
            return RawPath.GetHashCode();
        }

        public override string ToString()
        {
            return Path.ToString();
        }

        
    }
}
