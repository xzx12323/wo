using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeWorkDemo
{
    /// <summary>
    /// 请求
    /// </summary>
    public class RequestUrl
    {
        // https://item.jd.com/71059216734.html

        /// <summary>
        /// 根据url 请求网页，返回信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="contenttype"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string HttpGet(SetInfo info, string method = "GET", string contenttype = "application/json;charset=utf-8", Hashtable header = null, string data = null)
        {
            string msg = "";
            try
            {
                // HttpWebRequest 访问网络
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(info.url);
                request.Method = string.IsNullOrEmpty(method) ? "GET" : method;
                request.Timeout = 1000 * 60;
                request.ContentType = string.IsNullOrEmpty(contenttype) ? "application/json;charset=utf-8" : contenttype;
                if (header != null)
                {
                    //传方法头
                    foreach (var i in header.Keys)
                    {
                        request.Headers.Add(i.ToString(), header[i].ToString());
                    }
                }
                if (!string.IsNullOrEmpty(data))
                {
                    //传方法体
                    Stream RequestStream = request.GetRequestStream();
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    RequestStream.Write(bytes, 0, bytes.Length);
                    RequestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if ((int)response.StatusCode == 200)
                {
                    msg = info.title + "|" + info.url + " ,StatusCode :" + (int)response.StatusCode + " ,请求成功！";
                }
                else
                {
                    msg = info.title + "|" + info.url + " ,StatusCode :" + (int)response.StatusCode + " ,请求失败！";
                }

                #region 不需要
                //Stream ResponseStream = response.GetResponseStream();
                //StreamReader StreamReader = new StreamReader(ResponseStream, Encoding.GetEncoding("utf-8"));
                //string re = StreamReader.ReadToEnd();
                //StreamReader.Close();
                //ResponseStream.Close();
                #endregion


            }
            catch (UriFormatException)
            {
                msg = info.title + "|" + info.url + " ,StatusCode :" + "404" + " ,没有找到该网址！";
            }
            catch (TimeoutException)
            {
                msg = info.title + "|" + info.url + " ,StatusCode :" + "504" + " ,请求超时！";
            }
            catch (Exception ex)
            {
                if (ex.Message == "操作超时")
                {
                    msg = info.title + "|" + info.url + " ,StatusCode :" + "504" + " ,操作超时！";
                }
            }

            return msg;
        }

    }
}
