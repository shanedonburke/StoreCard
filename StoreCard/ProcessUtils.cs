using System.Diagnostics;

namespace StoreCard
{
    internal class ProcessUtils
    {
        public static void OpenInDefaultProgram(string path)
        {
            using Process openProcess = new Process();

            openProcess.StartInfo.FileName = "explorer";
            openProcess.StartInfo.Arguments = $"\"{path}\"";
            openProcess.Start();
        }
    }
}
