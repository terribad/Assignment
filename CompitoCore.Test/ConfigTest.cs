using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssignmentCore;
using System.IO;
namespace CompitoCore.Test
{
    [TestClass]
    public class ConfigTest
    {

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            VirtualPath.SetDefaultTemplates();
        }
        

        [TestMethod]
        public void CheckOnFileLoaded()
        {
            var c = new Config("config.ini");
            Assert.IsFalse(c.IsDefaultConfig, "Dovrebbe avere valori caricati");

            var expected = "rinominami";
            Assert.AreEqual(expected, c.ProjectName.ToLower(), "Project name dovrebbe essere {0}", expected);

            expected = @"d:\__compito\compito";
            Assert.AreEqual(expected, c.SourcePath.Path.ToLower(), "SourcePath dovrebbe essere {0}", expected);

            expected = Global.Desktop.ToLower();
            Assert.AreEqual(expected, c.DestPath.Path.ToLower(), "DestPath dovrebbe essere {0}", expected);

            expected = Global.CompleteAssignment.ToLower();
            Assert.AreEqual(expected, c.AssignmentPath.Path.ToLower(), "Assignment dovrebbe essere {0}", expected);            

            Assert.AreEqual(2, c.ClearFolders.Length, "Dovrebbero esistere 2 pre clear folders");
        }

        [TestMethod]
        public void CheckOnCompleteFileLoaded()
        {
            var c = new Config("configComplete.ini");
            Assert.IsFalse(c.IsDefaultConfig, "Dovrebbe avere valori caricati");

            var expected = "rossi";
            Assert.AreEqual(expected, c.ProjectName.ToLower(), "Project name dovrebbe essere {0}", expected);

            expected = Global.Projects.ToLower();
            Assert.AreEqual(expected, c.SourcePath.Path.ToLower(), "SourcePath dovrebbe essere {0}", expected);

            expected = @"d:\__compito\elaborati";
            Assert.AreEqual(expected, c.DestPath.Path.ToLower(), "DestPath dovrebbe essere {0}", expected);

            Assert.AreEqual(2, c.ClearFolders.Length, "Dovrebbero esistere 2 pre clear folders");
        }
    }
}
