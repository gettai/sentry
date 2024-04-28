using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaiSentry
{
    /// <summary>
    /// 启动参数管理器
    /// </summary>
    public class StartupParams
    {
        private static Dictionary<string, string> _startupParams = new Dictionary<string, string>();
        /// <summary>
        /// 获取一个指定参数值
        /// </summary>
        /// <param name="key_">参数名</param>
        /// <returns>不存在时返回null，存在时返回参数值字符串</returns>
        public static string? Get(string key_)
        {
            if (_startupParams.ContainsKey(key_))
            {
                return _startupParams[key_];
            }
            return null;
        }

        public static void Load(string[] args_)
        {
            for (int i = 0; i < args_.Length; i++)
            {
                if (args_[i].StartsWith("-"))
                {
                    string str = args_[i].Substring(1);
                    if (str.Contains(":"))
                    {
                        string[] strs = str.Split(':');
                        if (_startupParams.ContainsKey(strs[0]))
                        {
                            _startupParams[strs[0]] = strs[1];
                        }
                        else
                        {
                            _startupParams.Add(strs[0], strs[1]);
                        }
                    }
                    else
                    {
                        if (_startupParams.ContainsKey(str))
                        {
                            _startupParams[str] = string.Empty;
                        }
                        else
                        {
                            _startupParams.Add(str, string.Empty);
                        }
                    }
                }
            }
        }
    }
}
