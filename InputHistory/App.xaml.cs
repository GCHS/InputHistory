using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InputHistory {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
		readonly KeyboardListener KListener = new();

    private void Application_Startup(object sender, StartupEventArgs e) {
      KListener.KeyDown += new RawKeyEventHandler((s, e) => { if(MainWindow is not null) ((MainWindow)MainWindow).KListenerKeyDown(s, e); });
      KListener.KeyUp += new RawKeyEventHandler((s, e) => { if(MainWindow is not null) ((MainWindow)MainWindow).KListenerKeyUp(s, e); });
    }

    private void Application_Exit(object sender, ExitEventArgs e) {
      KListener.Dispose();
    }
  }
}