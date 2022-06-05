using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MapTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK6f516a63c76f44eca5c53507d8296f0cAVB6YF4ob_b9Tt_lzLK5nYYS_omZykJdQAX727mxRz7YZGWGC34b_YHYWKrfNV3p";
        }
    }
}
