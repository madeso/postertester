using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostTestr
{
    using System.Windows.Input;

    public static class CustomCommands
    {
        public static RoutedCommand Exit = new RoutedCommand();
        public static RoutedCommand NewRequest = new RoutedCommand();
        public static RoutedCommand Execute = new RoutedCommand();
        public static RoutedCommand Format = new RoutedCommand();
        public static RoutedCommand DeleteSelectedRequest = new RoutedCommand();
    }
}
