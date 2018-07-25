 
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Common
{
    /// <summary>
    /// 使用log4net记录日志
    /// </summary>
    public class Log4NetLoger : ILog
    {
        public Log4NetLoger()
        {

            XmlConfigurator.ConfigureAndWatch(new FileInfo(Setting.ConfigFilePath));
        }
         
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        /// 设置默认的监控超时阀值,单位毫秒
        /// </summary>
        public static uint DefaultMonitorThresholdTick = 5000;        //默认超时5秒的记录超时日志


        /// <summary>
        /// 记录Debug日志
        /// </summary>
        /// <param name="info"></param>
        public void WriteDebugLog(string info)
        {
            if (loginfo.IsDebugEnabled)
            {
                loginfo.Debug(info);
            }
        }

        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="info"></param>
        public void WriteInfoLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="info"></param>
        public void WriteErrorLog(string info)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info);
            }
        }

        /// <summary>
        /// 记录日志,该日志级别会记录为Info
        /// </summary>
        /// <param name="info">提示信息</param>
        public void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        /// <summary>
        /// 记录日志,该日志级别会被记录为error
        /// </summary>
        /// <param name="info">提示信息</param>
        /// <param name="se">错误</param>
        public void WriteLog(string info, Exception se)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, se);
            }
        }

        /// <summary>
        /// 记录日志,该日志级别会被记录为error
        /// </summary>
        /// <param name="se">错误</param>
        public void WriteLog(Exception se)
        {
            if (logerror.IsErrorEnabled)
            {
                 
                    try
                    {
                        var err = se;
                        string str = JsonConvert.SerializeObject(new { Message = err.Message });
                        logerror.Error(str, se);
                    }
                    catch
                    {
                        logerror.Error(string.Empty, se);
                    }
                
            }
        }

        /// <summary>
        /// 记录监控运行时长日志
        /// </summary>
        /// <param name="action">执行的action</param>
        /// <param name="actionName">执行action的名称描述</param>
        /// <param name="thresholdTick">超时阀值</param>
        public void MonitorTime(Action action, string actionName, uint thresholdTick)
        {
            if (action == null)
            {
                return;
            }

            var tempRemark = string.IsNullOrWhiteSpace(actionName) ? action.ToString() : actionName;

            var tempBegin = Environment.TickCount;
            var tempEnd = tempBegin;
            try
            {
                action();
                tempEnd = Environment.TickCount;
            }
            catch
            {
                tempEnd = Environment.TickCount;
                throw;
            }
            finally
            {
                var tickVal = tempEnd - tempBegin;
                if (tickVal > thresholdTick)
                {
                    loginfo.Info(string.Format("执行 {0} ,耗时: {1} 毫秒", actionName, tickVal));
                }
            }
        }

        /// <summary>
        /// 记录监控运行时长日志,超时阀值使用默认值
        /// </summary>
        /// <param name="action">执行的action</param>
        /// <param name="actionName">执行action的名称描述</param>
        public void MonitorTime(Action action, string actionName)
        {
            MonitorTime(action, actionName, DefaultMonitorThresholdTick);
        }
    }
}