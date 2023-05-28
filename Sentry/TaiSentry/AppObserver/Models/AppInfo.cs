using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiSentry.AppObserver.Enums;

namespace TaiSentry.AppObserver.Models
{
    /// <summary>
    /// 应用信息
    /// </summary>
    public class AppInfo
    {
        /// <summary>
        /// 进程ID
        /// </summary>
        public int PID { get; }
        /// <summary>
        /// 应用类型
        /// </summary>
        public AppType Type { get; }
        /// <summary>
        /// 进程名称
        /// </summary>
        public string Process { get; }
        /// <summary>
        /// 应用描述
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string ExecutablePath { get; }
        public AppInfo(int pid_, string process_, string description_, string executablePath_, AppType type_ = AppType.Win32)
        {
            PID = pid_;
            Process = process_;
            Description = description_;
            ExecutablePath = executablePath_;
            Type = type_;
        }

        public override string ToString()
        {
            return $"PID:{PID},ProcessName:{Process},Description:{Description},ExecutablePath:{ExecutablePath},Type:{Type}";
        }

        public static AppInfo Empty = new AppInfo(0, string.Empty, string.Empty, string.Empty);
    }
}
