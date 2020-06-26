﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PostTestr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var data = Disk.LoadOrCreateNew();
            var window = new MainWindow { DataContext = data, Data = data };
            window.Closed += (closedSender, closedArgs) => Disk.Save(data);
            window.OnSave = () => Disk.Save(data);
            window.Show();
        }
    }
}
