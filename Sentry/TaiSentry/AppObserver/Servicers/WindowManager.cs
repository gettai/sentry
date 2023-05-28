﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Models;
using TaiSentry.Utils.Win32;

namespace TaiSentry.AppObserver.Servicers
{
    public class WindowManager : IWindowManager
    {
        public WindowInfo GetWindowInfo(IntPtr handle_)
        {
            try
            {
                string title = GetWindowTitle(handle_);
                var rect = GetWindowRect(handle_);

                int width = rect.Width;
                int height = rect.Height;
                int x = rect.Left;
                int y = rect.Top;

                return new WindowInfo(title, handle_, width, height, x, y);
            }
            catch (Exception e)
            {
                return WindowInfo.Empty;
            }

        }

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="handle_"></param>
        /// <returns></returns>
        private string GetWindowTitle(IntPtr handle_)
        {
            try
            {
                int titleCapacity = Win32WindowAPI.GetWindowTextLength(handle_) * 2;
                StringBuilder stringBuilder = new StringBuilder(titleCapacity);
                Win32WindowAPI.GetWindowText(handle_, stringBuilder, stringBuilder.Capacity);
                string title = stringBuilder.ToString();
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        private Win32WindowAPI.RECT GetWindowRect(IntPtr handle_)
        {
            try
            {
                Win32WindowAPI.GetWindowRect(handle_, out Win32WindowAPI.RECT rect);
                return rect;

            }
            catch (Exception e)
            {
                return new Win32WindowAPI.RECT()
                {
                    Left = 0,
                    Bottom = 0,
                    Right = 0,
                    Top = 0
                };
            }
        }


    }
}
