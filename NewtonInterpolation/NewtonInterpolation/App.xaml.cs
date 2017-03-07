using NewtonInterpolation.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NewtonInterpolation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            var view = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            view.Show();
            base.OnStartup(e);
        }
    }
}
