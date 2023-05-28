﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Models;

namespace TaiSentry.AppObserver.Servicers
{
    public interface IAppManager
    {
        /// <summary>
        /// 通过窗口句柄获取应用信息
        /// </summary>
        /// <param name="hwnd_">窗口句柄</param>
        /// <returns></returns>
        AppInfo GetAppInfo(IntPtr hwnd_);
    }
}
