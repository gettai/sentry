using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaiSentry;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.Server;
using TaiSentry.Servicer;
using TaiSentry.StateObserver.Servicers;

namespace TaiSentryService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static IHost? AppHost { get; private set; }
        private Mutex _mutex;
        private Main _main;
        public App()
        {
            _main = new Main();
            //AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            //{
            //    services.AddSingleton<IAppManager, AppManager>();
            //    services.AddSingleton<IWindowManager, WindowManager>();
            //    services.AddSingleton<IAppObserver, AppObserver>();
            //    services.AddSingleton<IAppTimerServicer, AppTimerServicer>();
            //    services.AddSingleton<IStateObserverServicer, StateObserverServicer>();
            //    services.AddSingleton<IWSServer, WSServer>();
            //    services.AddSingleton<IMain, Main>();
            //}).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            PreventMultipleInstances();
            await _main!.Start(e.Args);
            //await AppHost!.StartAsync();
            if (e.Args.Length > 0)
            {
                MessageBox.Show(e.Args[0]);
            }
            //var main = AppHost.Services.GetRequiredService<IMain>();
            //main.Start();

            MainWindow = null;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            base.OnStartup(e);
        }

        #region 阻止服务多次启动
        /// <summary>
        /// 阻止服务多次启动
        /// </summary>
        private void PreventMultipleInstances()
        {
            _mutex = new Mutex(true, nameof(TaiSentryService), out bool isCreated);
            if (!isCreated)
            {
                Environment.Exit(0);
            }
        }
        #endregion

        protected override async void OnExit(ExitEventArgs e)
        {
            await _main.Stop();
            //await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
