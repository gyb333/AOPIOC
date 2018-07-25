using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Setting
    {
        static Setting()
        {
            var date = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:00:00");
            DMS_CLIENT_ID = Globals.Configuration.AppSettings.Settings["DMS_CLIENT_ID"].Value;
            DMS_CLIENT_SECRET = Globals.Configuration.AppSettings.Settings["DMS_CLIENT_SECRET"].Value;
            DMS_API_URL = Globals.Configuration.AppSettings.Settings["DMS_API_URL"].Value;
            FetchPageSize = int.Parse(Globals.Configuration.AppSettings.Settings["FetchPageSize"].Value);
            DMSTransferConnectString = Globals.Configuration.ConnectionStrings.ConnectionStrings["DMSTransfer"].ConnectionString;
            Kds2ConnectString = Globals.Configuration.ConnectionStrings.ConnectionStrings["Kds2"].ConnectionString;
            SIGN_METHOD = Globals.Configuration.AppSettings.Settings["SIGN_METHOD"].Value;
            FetchDate = date;
            CommandTimeout = int.Parse(Globals.Configuration.AppSettings.Settings["CommandTimeout"].Value);
            ConfigFilePath = Globals.Configuration.FilePath;
            MaxTryFetchTimes = int.Parse(Globals.Configuration.AppSettings.Settings["MaxTryFetchTimes"].Value);
            MaxReTryFetchDays = int.Parse(Globals.Configuration.AppSettings.Settings["MaxReTryFetchDays"].Value);

        }
        public static void SetFetchDate(DateTime fetchDate)
        {
            Setting.FetchDate = fetchDate;
        }

        public static string ConfigFilePath { get; }
        public static string DMS_CLIENT_ID { get; }
        public static string DMS_CLIENT_SECRET { get; }
        public static string DMS_API_URL { get; }
        public static int FetchPageSize { get; }
        public static string DMSTransferConnectString { get; }
        public static string Kds2ConnectString { get; }
        public static string SIGN_METHOD { get; }
        public static DateTime FetchDate { get; private set; }
        public static int FetchDays { get; private set; }
        public static int CommandTimeout { get; }

        /// <summary>
        /// 如果本次失败尝试抓取的次数
        /// </summary>
        public static int MaxTryFetchTimes { get; }

        /// <summary>
        /// 接口抓取失败 最大尝试获取天数
        /// </summary>
        public static int MaxReTryFetchDays { get; }

    }
}
