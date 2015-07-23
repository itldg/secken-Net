using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Web.Security;
using System.Net;
using System.Text;
using System.IO;

namespace DiyCms.CmsAdmin
{
    ///<summary>

    ///类说明：洋葱验证类库

    ///作用：实现对洋葱的各个操作（如获取登录二维码，绑定账号等）

    ///作者：王龙（该类在diycms.com中应用测试并完善）
    
    ///联系QQ:78847023 Email:wanglong126@139.com

    ///编写日期：2015-07-18  最后更新日期：2015-07-23

    ///API指导：xiaochao 意见改进：gitxpj

    ///API说明地址：https://www.yangcong.com/api
    

    ///</summary>
    public class Secken
    {
     #region 洋葱配置
    /// <summary>
    /// 应用ID
    /// </summary>
     public  string _APP_ID = "你的APPID";
    /// <summary>
    /// 应用Key
    /// </summary>
    public  string _APP_KEY = "你的APPKEY";
    /// <summary>
    /// web授权code
    /// </summary>
    public  string _Auth_ID = "你的AUTHID";
    /// <summary>
    /// 获取绑定二维码
    /// </summary>
    public static string _GetBindingCode = "https://api.yangcong.com/v2/qrcode_for_binding";
    /// <summary>
    /// 获取登录二维码
    /// </summary>
    public static string _GetLoginCode = "https://api.yangcong.com/v2/qrcode_for_auth";
    /// <summary>
    /// 查询UUID事件结果
    /// </summary>
    public static string _GetResult = "https://api.yangcong.com/v2/event_result";

    /// <summary>
    /// 一键认证
    /// </summary>
    public static string _VerifyOneClick = "https://api.yangcong.com/v2/realtime_authorization";

    /// <summary>
    /// 动态码验证
    /// </summary>
    public static string _VerifyOTP = "https://api.yangcong.com/v2/offline_authorization";

    /// <summary>
    /// 洋葱网授权页
    /// </summary>
    public static string _AuthPage = "https://auth.yangcong.com/v2/auth_page";
#endregion

     #region 洋葱操作

    public  Secken(string appid, string appkey, string authid, string callback="")
    {
        _APP_ID = appid;
        _APP_KEY = appkey;
        _Auth_ID = authid;
    }
    /// <summary>
    /// 获取绑定二维码
    /// </summary>
    /// <returns>获得返回的json数据</returns>
    public  string getBindingCode(string callback="")
    {
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile("app_id=" + _APP_ID + (callback.Length == 0 ? "" : "callback=" + HttpUtility.UrlEncodeUnicode(callback)) + _APP_KEY, "MD5");
        string data = "app_id=" + _APP_ID + (callback.Length == 0 ? "" :"&callback=" + HttpUtility.UrlEncodeUnicode(HttpUtility.UrlEncodeUnicode(callback))) + "&signature=" + md5;
        string result = Http(_GetBindingCode, "GET", data);
        return result;
    }

    /// <summary>
    /// 获取登录二维码
    /// </summary>
    /// <returns>获得返回的json数据</returns>
    public string getLoginCode(string callback = "")
    {
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile("app_id=" + _APP_ID + (callback.Length == 0 ? "" : "callback=" + HttpUtility.UrlEncodeUnicode(callback)) + _APP_KEY, "MD5");
        string data = "app_id=" + _APP_ID + (callback.Length == 0 ? "" : "&callback=" + HttpUtility.UrlEncodeUnicode(HttpUtility.UrlEncodeUnicode(callback))) + "&signature=" + md5;
        string result = Http(_GetLoginCode, "GET", data);
        return result;
    }
    /// <summary>
    /// 无网络认证
    /// </summary>
    /// <param name="callback">回调地址</param>
    /// <returns>获得返回的json数据</returns>
    public  string authPage(string callback)
    {
        string timedata = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        string temp = "auth_id=" + _Auth_ID + "callback=" + HttpUtility.UrlEncodeUnicode(callback) + "timestamp=" + timedata + _APP_KEY;
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(temp, "MD5").ToLower();
        string data = "auth_id=" + _Auth_ID + "&timestamp=" + timedata + "&callback=" + HttpUtility.UrlEncode(HttpUtility.UrlEncode(callback)) + "&signature=" + md5;
        return _AuthPage + "?" + data;
    }
    /// <summary>
    /// 向洋葱手机App推送一条实时验证
    /// </summary>
    /// <param name="userid">用户ID</param>
    /// <param name="user_ip">用户IP</param>
    /// <param name="username">用户名或用户称呼 默认是用户</param>
    /// <param name="action_type">操作类型 默认1 1(登录验证)、2(支付验证)、3(交易验证)、4(其他验证)</param>
    /// <param name="auth_type">验证类型 默认1 1(点击确定按钮)、2(使用手势密码)、3(人脸验证)、4(声音验证)</param>
    /// <returns></returns>
    public  string verifyOneClick(string userid, string user_ip, string username = "用户", int action_type = 1, int auth_type = 1,string callback="")
    {
        string temp = "action_type=" + action_type + "app_id=" + _APP_ID + "auth_type=" + auth_type +(callback.Length == 0 ? "" : "callback=" + HttpUtility.UrlEncodeUnicode(callback)) + "uid=" + userid + "user_ip=" + user_ip + "username=" + username + _APP_KEY;
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(temp, "MD5").ToLower();
        string data = "action_type=" + action_type + "&app_id=" + _APP_ID + "&auth_type=" + auth_type + (callback.Length == 0 ? "" : "&callback=" + HttpUtility.UrlEncodeUnicode(HttpUtility.UrlEncodeUnicode(callback)))  + "&uid=" + userid + "&user_ip=" + user_ip + "&username=" + username + "&signature=" + md5;
        string result = Http(_VerifyOneClick, "POST", data);
        return result;
    }
    public  string verifyOneClick(string userid, int action_type = 1, int auth_type = 1)
    {
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile("action_type=" + action_type + "app_id=" + _APP_ID + "auth_type=" + auth_type + "uid=" + userid + _APP_KEY, "MD5").ToLower();

        string data = "action_type=" + action_type + "&app_id=" + _APP_ID + "&auth_type=" + auth_type + "&uid=" + userid + "&signature=" + md5;
        string result = Http(_VerifyOneClick, "POST", data);
        return result;
    }
    /// <summary>
    /// 绑定账号之后，动态验证
    /// </summary>
    /// <param name="uid">用户编号</param>
    /// <param name="dynamic_code">动态密码</param>
    /// <returns></returns>
    public  string verifyOTP(string uid, int dynamic_code)
    {
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile("app_id=" + _APP_ID + "dynamic_code=" + dynamic_code + "uid=" + uid + _APP_KEY, "MD5").ToLower();
        string data = "app_id=" + _APP_ID + "&uid=" + uid + "&dynamic_code=" + dynamic_code + "&signature=" + md5;
        string result = Http(_VerifyOTP, "POST", data);
        return result;
    }
    /// <summary>
    /// 查看event_id所对应的事件响应结果，该 event_id 可以通过 /v2/realtime_authorization、/v2/qrcode_for_auth、/v2/qrcode_for_binding 获得,此操作为异步获取等待用户扫码产生响应或直至接口返回超时不可重试,返回结果之后，必须进行签名验证，避免传输过程被恶意修改。event_id有效时间为 300s。
    /// </summary>
    /// <param name="event_id">事件 id，一个20字节的字符创，用来标识某个洋葱认证事件。</param>
    /// <returns>获得返回的json数据</returns>
    public  string getResult(string event_id)
    {
        string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile("app_id=" + _APP_ID + "event_id=" + event_id + _APP_KEY, "MD5");
        string data = "app_id=" + _APP_ID + "&event_id=" + event_id + "&signature=" + md5;
        string result = Http(_GetResult, "GET", data);
        return result;
    }
    /// <summary>
    /// 检验返回签名
    /// </summary>
    /// <param name="json">网页返回的json</param>
    /// <returns>返回是否校验通过</returns>
    public bool Check(string json)
    {
        LitJson.JsonData jd = LitJson.JsonMapper.ToObject(json);
        try
        {
		    IDictionary tdictionary = jd as IDictionary;
            StringBuilder sb = new StringBuilder();
            foreach (System.Collections.DictionaryEntry name in tdictionary)
              {
                  if (name.Key.ToString()!="signature")
                  {
                      sb.Append(name.Key.ToString()+"="+jd[name.Key.ToString()].ToString());
                  }  
              }
            string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(sb.ToString()+_APP_KEY, "MD5").ToLower();
            if (md5 == jd["signature"].ToString())
            {
                return true;
            }
	    }
	    catch (Exception)
	    {
		
		    return false;
	    }
        

        return false;
    }
    #endregion
   
     #region 网络配置
        /// <summary>
        /// GET请求与获取结果
        /// </summary>
        public static string Http(string Url, string method,string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = method;
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode!=HttpStatusCode.OK)
            {

                return "{\"Description\":\"" + response.StatusDescription + "\",\"Status\":500}";
            }
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
#endregion

    }
}
