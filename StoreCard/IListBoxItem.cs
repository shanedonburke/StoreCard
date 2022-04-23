using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StoreCard
{
    public interface IListBoxItem
    {
        public string Name { get; }

        public ImageSource BitmapIcon { get; }
        
        public ImageSource? PrefixIcon { get; }
    }
}
