using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Servicers;
using TaiSentry.AppTimer.Servicers;
using TaiSentry.Notification;
using TaiSentry.Server;
using TaiSentry.Servicer;
using TaiSentry.StateObserver.Servicers;

namespace TaiSentry
{
    /// <summary>
    /// TaiSentry入口文件
    /// </summary>
    public class Main
    {
        public static IHost? AppHost { get; private set; }
        private IMainServicer _mainServicer;
        public Main()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IAppManager, AppManager>();
                services.AddSingleton<IWindowManager, WindowManager>();
                services.AddSingleton<IAppObserver, TaiSentry.AppObserver.Servicers.AppObserver>();
                services.AddSingleton<IAppTimerServicer, AppTimerServicer>();
                services.AddSingleton<IStateObserverServicer, StateObserverServicer>();
                services.AddSingleton<IWSServer, WSServer>();
                services.AddSingleton<IMainServicer, MainSeriver>();
                services.AddSingleton<IStateManagerServicer, StateManagerServicer>();
                services.AddSingleton<IAppTimerManagerServicer, AppTimerManagerServicer>();
                services.AddSingleton<ISubscriberManager, SubscriberManager>();
                services.AddSingleton<IAppObserverManagerServicer, AppObserverManagerServicer>();
                services.AddSingleton<IWSServerManagerServicer, WSServerManagerServicer>();
            }).Build();
        }

        public async Task Start(string[] args_)
        {
            await AppHost!.StartAsync();
            StartupParams.Load(args_);
            _mainServicer = AppHost.Services.GetRequiredService<IMainServicer>();
            _mainServicer.Start();
        }

        public async Task Stop()
        {
            _mainServicer.Stop();
            await AppHost!.StopAsync();
        }
    }
}
