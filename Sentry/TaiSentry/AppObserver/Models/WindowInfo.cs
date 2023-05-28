﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry.AppObserver.Models
{
    /// <summary>
    /// 窗口信息
    /// </summary>
    public class WindowInfo
    {
        /// <summary>
        /// 窗口标题
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IntPtr Handle { get; }
        /// <summary>
        /// 窗口宽度
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// 窗口高度
        /// </summary>
        public int Height { get; }
        public int X { get; }
        public int Y { get; }

        public WindowInfo(string title_, IntPtr handle_, int width_, int height_, int x_, int y_)
        {
            Title = title_;
            Handle = handle_;
            Width = width_;
            Height = height_;
            X = x_;
            Y = y_;
        }

        public override string ToString()
        {
            return $"Title:{Title},Handle:{Handle},Width:{Width},Height:{Height},X:{X},Y:{Y}";
        }
        public static WindowInfo Empty = new WindowInfo(string.Empty, IntPtr.Zero, 0, 0, 0, 0);
    }
}
