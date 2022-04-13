using System;
using System.Diagnostics;
using System.Linq;

namespace StoreCard
{
    public abstract class SavedFileSystemItem : SavedItem
    {
        public static string DEFAULT_EXECUTABLE = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\explorer.exe";

        public readonly string Path;

        public string ExecutablePath { get; private set; }

        public string ExecutableName =>
            ExecutablePath == DEFAULT_EXECUTABLE ? "Default" : ExecutablePath.Split("/").Last();

        protected SavedFileSystemItem(string id, string name, string? base64Icon, string path, string executablePath)
            : base(id, name, base64Icon)
        {
            Path = path;
            ExecutablePath = executablePath;
        }

        public override void Open()
        {
            using Process openProcess = new Process();

            openProcess.StartInfo.FileName = ExecutablePath;
            openProcess.StartInfo.Arguments = $"\"{Path}\"";
            openProcess.Start();
        }
    }
}
