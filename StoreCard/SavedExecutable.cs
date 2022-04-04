using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StoreCard
{
    internal class SavedExecutable : SavedItem
    {
        public string Path { get; private set; }

        public SavedExecutable(string name, string? base64Icon, string path) : base(name, base64Icon)
        {
            Path = path;
        }

        public override void Open()
        {
            Process.Start(Path);
        }
    }
}
