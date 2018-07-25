using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 获取数据的参数选项配置
    /// </summary>
    public class FetchOption
    {
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="reportType"></param>
        /// <param name="pageNo"></param>
        private FetchOption(int pageNo)
        {
            int fetchDays = 1;
            DateTime fetchDate = Setting.FetchDate;
            UpdateTimeStart = fetchDate.AddDays(-fetchDays).ToString("yyyy-MM-dd HH:mm:ss");
            UpdateTimeEnd = fetchDate.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
            this.PageNo = pageNo;
            this.PageSize = Setting.FetchPageSize;
       
        }
        private FetchOption( DateTime fetcBeginhDate, DateTime fetchEndDate, int pageNo)
        {
            DateTime fetchDate = Setting.FetchDate;
            UpdateTimeStart = fetcBeginhDate.ToString("yyyy-MM-dd HH:mm:ss");
            UpdateTimeEnd = fetchEndDate.ToString("yyyy-MM-dd HH:mm:ss");
            this.PageNo = pageNo;
            this.PageSize = Setting.FetchPageSize;

        }
        /// <summary>
        /// 根据报表类型和当前页生成报表请求配置
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public static FetchOption Create( int pageNo)
        {
            FetchOption option = new FetchOption( pageNo);
            return option;
        }

        internal static FetchOption Create( DateTime fetcBeginhDate, DateTime fetchEndDate, int pageNo)
        {
            FetchOption option = new FetchOption( fetcBeginhDate, fetchEndDate, pageNo);
            return option;
        }
        /// <summary>
        /// 报表编码
        /// </summary>
        public string ReportCode { get; set; }
        /// <summary>
        /// 响应结果是否加密压缩，1表示开启,缺省不开启.
        ///        加密算法: (1). GZIP压缩;
        ///(2). DES加密, key指定为client_secret;
        ///(3). Base64编码
        ///对响应数据的解密为上述过程的逆向，即：
        ///(1). Base64解码
        ///(2). DES解密, key指定为client_secret;
        ///(3). GZIP解压缩.

        /// </summary>
        public string Epcp { get; set; }
        /// <summary>
        /// 报表结果集输出格式:(1),10表示输出结果为json/mapping格式(默认)
        /// </summary>
        public string Output { get; } = "10";
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNo { get; }
        /// <summary>
        /// 当前页码大小
        /// </summary>
        public int PageSize { get; }
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public string UpdateTimeStart { get; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public string UpdateTimeEnd { get; }

        private RequestInfo _info;

        /// <summary>
        /// 请求信息 包含已经签名
        /// </summary>
        public RequestInfo RequestInfo
        {

            get
            {
                if (_info == null)
                {

                    Dictionary<String, String> pars = new Dictionary<string, string>();
                    pars.Add("output", Output);
                    pars.Add("updateTimeStart", UpdateTimeStart);
                    pars.Add("updateTimeEnd", UpdateTimeEnd);
                    /** 公共参数. */
                    pars.Add("client_id", Setting.DMS_CLIENT_ID);
                    pars.Add("sign_method", Setting.SIGN_METHOD);
                    pars.Add("timestamp", SignUtils.CurrentTimeMillis.ToString());

                    /** 带签名执行请求 */
                    pars.Add("winc_sign", SignUtils.SignWcopRequest(pars, Setting.DMS_CLIENT_SECRET, Setting.SIGN_METHOD));

                    string url = string.Format(Setting.DMS_API_URL,"Test");
                    _info = new RequestInfo(url, pars);
                    return _info;
                }
                return _info;
            }
        }


    }
}
