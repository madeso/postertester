namespace PosterTester
{
    using System.Windows.Input;

    public static class CustomCommands
    {
        public static RoutedCommand Ok = new RoutedCommand();

        public static RoutedCommand Save = new RoutedCommand();
		public static RoutedCommand BrowseData = new RoutedCommand();
		public static RoutedCommand Exit = new RoutedCommand();
		public static RoutedCommand Settings = new RoutedCommand();

		public static RoutedCommand CreateNewGroup = new RoutedCommand();
        public static RoutedCommand AddExistingGroup = new RoutedCommand();
        public static RoutedCommand ForgetGroup = new RoutedCommand();
		public static RoutedCommand GroupSettings = new RoutedCommand();

		public static RoutedCommand NewRequest = new RoutedCommand();
        public static RoutedCommand Execute = new RoutedCommand();
		public static RoutedCommand Attack = new RoutedCommand();
		public static RoutedCommand Format = new RoutedCommand();
        public static RoutedCommand DeleteSelectedRequest = new RoutedCommand();
        public static RoutedCommand LoadPost = new RoutedCommand();
        public static RoutedCommand Compare = new RoutedCommand();
		public static RoutedCommand CompareAttack = new RoutedCommand();
		public static RoutedCommand Rename = new RoutedCommand();

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

		public static RoutedCommand BrowseUrl = new RoutedCommand();
		public static RoutedCommand ChangeTimeout = new RoutedCommand();

	}
}
