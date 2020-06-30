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
        public static RoutedCommand Save = new RoutedCommand();
        public static RoutedCommand Exit = new RoutedCommand();
        public static RoutedCommand NewRequest = new RoutedCommand();
        public static RoutedCommand Execute = new RoutedCommand();
        public static RoutedCommand Format = new RoutedCommand();
        public static RoutedCommand DeleteSelectedRequest = new RoutedCommand();
        public static RoutedCommand TogglePost = new RoutedCommand();

        public static RoutedCommand SelectRequest1 = new RoutedCommand();
        public static RoutedCommand SelectRequest2 = new RoutedCommand();
        public static RoutedCommand SelectRequest3 = new RoutedCommand();
        public static RoutedCommand SelectRequest4 = new RoutedCommand();
        public static RoutedCommand SelectRequest5 = new RoutedCommand();
        public static RoutedCommand SelectRequest6 = new RoutedCommand();
        public static RoutedCommand SelectRequest7 = new RoutedCommand();
        public static RoutedCommand SelectRequest8 = new RoutedCommand();
        public static RoutedCommand SelectRequest9 = new RoutedCommand();

        public static RoutedCommand FocusRequests = new RoutedCommand();
        public static RoutedCommand FocusUrl = new RoutedCommand();
        public static RoutedCommand FocusPost = new RoutedCommand();
    }
}
