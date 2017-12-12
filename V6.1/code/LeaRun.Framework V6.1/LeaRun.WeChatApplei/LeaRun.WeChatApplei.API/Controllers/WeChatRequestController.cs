using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace LeaRun.WeChatApplei.API.Controllers
{
    public class WeChatRequestController : ApiController
    {
        /// <summary>
        /// 获取openid接口
        /// </summary>
        /// <param name="code">登录凭证</param>
        /// <returns></returns>
        public string GetOpenId(string code)
        {
            try
            {
                string url = "https://api.weixin.qq.com/sns/jscode2session?appid=wxb767dd0b358a325f&secret=13f2f7d16c3b0d41c3cbaa3af191aa7b&js_code=" + code + "&grant_type=authorization_code";
                System.Net.WebRequest wReq = HttpWebRequest.Create(url);
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {

            }
            return "";
        }


        /// <summary>
        /// 获取AccessToken接口
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetAccessToken()
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wxb767dd0b358a325f&secret=13f2f7d16c3b0d41c3cbaa3af191aa7b";
                System.Net.WebRequest wReq = HttpWebRequest.Create(url);
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {

            }
            return "";
        }


        /// <summary>
        /// 发送模板消息接口
        /// </summary>
        /// <param name="AccessToken">AccessToken（必填）</param>
        /// <param name="openid">openid（必填）</param>
        /// <param name="template_id">所需下发的模板消息的id（必填）</param>
        /// <param name="page">点击模板卡片后的跳转页面（可空）</param>
        /// <param name="form_id">form_id（必填）</param>
        /// <param name="data">模板内容（必填）</param>
        /// <param name="color">模板内容字体的颜色（可空）</param>
        /// <param name="emphasis_keyword">模板需要放大的关键词（可空）</param>
        /// <returns></returns>
        [HttpPost]
        public string SendTemplateMessage([FromBody]TemplateMessageData data)
        {
            try
            {
                string postData = "touser=" + data.touser;
                postData += "&template_id=" + data.template_id;
                postData += "&page=" + data.page;
                postData += "&form_id=" + data.form_id;
                postData += "&data=" + data.data;
                postData += "&color=" + data.color;
                postData += "&emphasis_keyword=" + data.emphasis_keyword;

                PostData pd = new PostData();
                pd.touser = data.touser;
                pd.template_id = data.template_id;
                pd.page = data.page;
                pd.form_id = data.form_id;
                pd.data = new JavaScriptSerializer().Deserialize(data.data, typeof(Data));
                /*
                pd.data = new Data();
                pd.data.keyword1 = new DataItem("339208499", "#173177");
                pd.data.keyword2 = new DataItem("2015年01月05日 12:30", "#173177");
                pd.data.keyword3 = new DataItem("粤海喜来登酒店", "#173177");
                pd.data.keyword4 = new DataItem("广州市天河区天河路208号", "#173177");
                 * */
                pd.color = data.color;
                pd.emphasis_keyword = data.emphasis_keyword;


                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] byteArray = encoding.GetBytes(new JavaScriptSerializer().Serialize(pd));

                string url = "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token=" + data.AccessToken;
                System.Net.WebRequest wReq = HttpWebRequest.Create(url);
                wReq.Method = "POST";
                wReq.ContentType = "application/x-www-form-urlencoded";
                //wReq.ContentLength = byteArray.Length;
                //请求流
                Stream dataStream = wReq.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                //获取响应数据
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {

            }
            return "";
        }


        public class TemplateMessageData
        {
            public string AccessToken { get; set; }
            public string touser { get; set; }
            public string template_id { get; set; }
            public string page { get; set; }
            public string form_id { get; set; }
            public string data { get; set; }
            public string color { get; set; }
            public string emphasis_keyword { get; set; }
        }

        public class PostData
        {
            public string touser { get; set; }
            public string template_id { get; set; }
            public string page { get; set; }
            public string form_id { get; set; }
            public object data { get; set; }
            public string color { get; set; }
            public string emphasis_keyword { get; set; }
        }

        public class Data
        {
            public DataItem keyword1 { get; set; }
            public DataItem keyword2 { get; set; }
            public DataItem keyword3 { get; set; }
            public DataItem keyword4 { get; set; }
            public DataItem keyword5 { get; set; }
            public DataItem keyword6 { get; set; }
            public DataItem keyword7 { get; set; }
            public DataItem keyword8 { get; set; }
            public DataItem keyword9 { get; set; }
            public DataItem keyword10 { get; set; }
        }

        public class DataItem
        {
            public string DATA { get; set; }
        }

    }
}
