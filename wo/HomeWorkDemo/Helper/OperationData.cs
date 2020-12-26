using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace HomeWorkDemo
{
    /// <summary>
    /// 操作存储的xml文件
    /// </summary>
    public class OperationData
    {
        //技术：对XML文件操作
        #region 用户相关
        /// <summary>
        /// 存储用户xml位置
        /// </summary>
        private static string _userxmlPath = Application.StartupPath + @"\USER.XML";
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public static void AddUserXml(string name, string password)
        {
            try
            {
                if (!File.Exists(_userxmlPath))//不存在xml文件，新增
                {
                    XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                     new XElement("UserInfo", ""));
                    xdoc.Save(_userxmlPath);//保存xml
                }
                XElement xe = XElement.Load(_userxmlPath);
                XElement record = new XElement
                (
                new XElement("User",
                new XAttribute("name", name),
                new XAttribute("password", password))
                );
                xe.Add(record);//新增xml节点和属性
                xe.Save(_userxmlPath);//保存xml
                MessageBox.Show("注册成功！");
            }
            catch
            {
                MessageBox.Show("注册失败！");
            }


        }
        /// <summary>
        /// 检查用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="mesg"></param>
        /// <returns></returns>
        public static bool CheckUser(string name, string password, ref string mesg)
        {
            if (!File.Exists(_userxmlPath))
            {
                mesg = "还未注册用户，请检查！";
                return false;
            }
            XElement xe = XElement.Load(_userxmlPath);

            IEnumerable<XElement> element = from ele in xe.Elements("User")
                                            where ele.Attribute("name").Value == name
                                            && ele.Attribute("password").Value == password
                                            select ele;//使用linq 技术根据账户密码验证是否存在用户信息
            if (element.Count() > 0)
            {
                mesg = "";
            }
            else
            {
                mesg = "用户或密码错误，请检查！";
                return false;
            }
            return true;
        }

        #endregion

        #region 配置相关
        /// <summary>
        /// 存储配置的xml地址
        /// </summary>
        private static string _setxmlPath = Application.StartupPath + @"\SET.XML";
        /// <summary>
        /// 获取xml内的配置信息
        /// </summary>
        /// <returns></returns>
        public static List<SetInfo> GetSetData()
        {
            if (!File.Exists(_setxmlPath))
            {
                return null;
            }
            XElement xe = XElement.Load(_setxmlPath);//加载xml文件
            IEnumerable<XElement> elements = from ele in xe.Elements("Set")//获取xml文件Set节点下的所有数据
                                             select ele;
            if (elements.Count() > 0)
            {
                List<SetInfo> modelList = new List<SetInfo>();
                foreach (var ele in elements)
                {//获取到xml节点的属性值依次赋值给 SetInfo 类
                    SetInfo model = new SetInfo();
                    model.title = ele.Attribute("title").Value;
                    model.time = ele.Attribute("time").Value;
                    model.rate = ele.Attribute("rate").Value;
                    model.interval = ele.Attribute("interval").Value;
                    model.memo = ele.Attribute("memo").Value;
                    model.enable = ele.Attribute("enable").Value;
                    model.url = ele.Attribute("url").Value;
                    modelList.Add(model);
                }
                return modelList;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 增加配置信息
        /// </summary>
        /// <param name="info"></param>
        public static void AddSetXml(SetInfo info)
        {
            try
            {
                if (!File.Exists(_setxmlPath))//使用文件流相关方法，判断文件存不存在
                {
                    XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                     new XElement("SetInfo", ""));
                    xdoc.Save(_setxmlPath);//保存xml文件
                }

                XElement xe = XElement.Load(_setxmlPath);//加载xml

                IEnumerable<XElement> elements = from ele in xe.Elements("Set")
                                                 where (string)ele.Attribute("title") == info.title
                                                 select ele;//使用 linq 技术 根据标题判断是否存在相同标题
                if (elements.Count() > 0)
                {
                    MessageBox.Show("当前标题已经存在，不允许新增，只能修改！");
                    return;
                }
                XElement record = new XElement
                (
                new XElement("Set",
                new XAttribute("title", info.title),
                new XAttribute("time", info.time),
                new XAttribute("rate", info.rate),
                new XAttribute("interval", info.interval),
                new XAttribute("memo", info.memo),
                new XAttribute("enable", info.enable),
                new XAttribute("url", info.url))
                );
                xe.Add(record);//xml增加节点
                xe.Save(_setxmlPath);
                MessageBox.Show("保存成功！");
            }
            catch
            {
                MessageBox.Show("保存失败！");
            }
        }
        /// <summary>
        /// 删除配置信息
        /// </summary>
        /// <param name="title"></param>
        public static void DelSetXml(string title)
        {
            try
            {
                XElement xe = XElement.Load(_setxmlPath);//加载xml
                IEnumerable<XElement> elements = from ele in xe.Elements("Set")
                                                 where (string)ele.Attribute("title") == title
                                                 select ele;
                {
                    if (elements.Count() > 0)
                        elements.First().Remove();//删除xml节点
                }
                xe.Save(_setxmlPath);
            }
            catch
            {
                MessageBox.Show("删除成功！");
            }
        }
        /// <summary>
        /// 修改配置信息
        /// </summary>
        /// <param name="info"></param>
        public static void UpdateSetXml(SetInfo info)
        {
            try
            {
                XElement xe = XElement.Load(_setxmlPath);
                IEnumerable<XElement> elements = from ele in xe.Elements("Set")
                                                 where (string)ele.Attribute("title") == info.title
                                                 select ele;//根据title 找到对应xml节点
                {
                    if (elements.Count() > 0)
                        elements.First().Remove();//删除节点
                }
         

                XElement record = new XElement
               (
               new XElement("Set",
               new XAttribute("title", info.title),
               new XAttribute("time", info.time),
               new XAttribute("rate", info.rate),
               new XAttribute("interval", info.interval),
               new XAttribute("memo", info.memo),
               new XAttribute("enable", info.enable),
               new XAttribute("url", info.url))
               );
                xe.Add(record);//新增节点
                xe.Save(_setxmlPath);
                MessageBox.Show("修改成功！");
            }
            catch
            {
                MessageBox.Show("修改失败！");
            }
        }

        #endregion
    }
}
