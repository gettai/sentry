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
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.StateObserver.Servicers;

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
                services.AddSingleton<IStateObserverServicer, StateObserverServicer>();
            }).Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            PreventMultipleInstances();

            await AppHost!.StartAsync();

            var service = AppHost.Services.GetRequiredService<IAppObserver>();
            var timerService = AppHost.Services.GetRequiredService<IAppTimerServicer>();
            var stateService = AppHost.Services.GetRequiredService<IStateObserverServicer>();
            service.Start();
            timerService.Start();
            stateService.Start();
            stateService.OnStateChanged += StateService_OnStateChanged;
            MainWindow = null;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            base.OnStartup(e);
        }

        private void StateService_OnStateChanged(object sender_, TaiSentry.StateObserver.Events.StateChangedEventArgs e_)
        {
            Debug.WriteLine("【状态变更】" + e_.Status);
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
