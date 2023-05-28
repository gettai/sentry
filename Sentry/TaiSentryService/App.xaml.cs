using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;

namespace TaiSentryService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        private Mutex _mutex;
        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IAppManager, AppManager>();
                services.AddSingleton<IWindowManager, WindowManager>();
                services.AddSingleton<IAppObserver, AppObserver>();
                services.AddSingleton<IAppTimerServicer, AppTimerServicer>();
            }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            PreventMultipleInstances();

            await AppHost!.StartAsync();

            var service = AppHost.Services.GetRequiredService<IAppObserver>();
            var timerService = AppHost.Services.GetRequiredService<IAppTimerServicer>();
            service.Start();
            timerService.Start();

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
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
