using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeWorkDemo
{
    public partial class ManagementForm : Form
    {
        public ManagementForm()
        {
            InitializeComponent();
            Refresh();
        }

        #region 消息传递
        //使用技术：委托
        /// <summary>
        /// 跨线程操作，防止卡界面
        /// </summary>
        /// <param name="text"></param>

        delegate void SetTextCallback(string text);

        private void ThreadProcSafe(string mes)
        {
            mes = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "|" + mes + System.Environment.NewLine;
            this.SetText(mes);

        }
        private void SetText(string text)
        {

            if (this.txtMessage.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.txtMessage.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.txtMessage.Disposing || this.txtMessage.IsDisposed)
                        return;
                }
                SetTextCallback d = new SetTextCallback(SetText);
                this.txtMessage.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtMessage.Text += text;
            }
        }
        #endregion

        #region 配置设置 Tap 页
        /// <summary>
        /// 获取配置
        /// </summary>
        private void Refresh()
        {
            List<SetInfo> setinfos = new List<SetInfo>();
            setinfos = OperationData.GetSetData();
            if (setinfos != null && setinfos.Count > 0)
            {
                DataTable dt = new DataTable();
                DataColumn c1 = new DataColumn("title");
                DataColumn c2 = new DataColumn("time");
                DataColumn c3 = new DataColumn("rate");
                DataColumn c4 = new DataColumn("interval");
                DataColumn c5 = new DataColumn("memo");
                DataColumn c6 = new DataColumn("url");
                DataColumn c7 = new DataColumn("enable");
                dt.Columns.Add(c1);
                dt.Columns.Add(c2);
                dt.Columns.Add(c3);
                dt.Columns.Add(c4);
                dt.Columns.Add(c5);
                dt.Columns.Add(c6);
                dt.Columns.Add(c7);

                foreach (SetInfo info in setinfos)
                {
                    dt.Rows.Add(new object[] { info.title, info.time, info.rate, 
                        info.interval, info.memo, info.url, info.enable});
                }
                dataGrid.DataSource = dt;
            }
            else
            {
                dataGrid.DataSource = null;
            }

        }
        /// <summary>
        /// 新增配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddorModifyForm frm = new AddorModifyForm(true);
            frm.ShowDialog();
            Refresh();
        }
        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (dataGrid.DataSource == null)
            {
                MessageBox.Show("请选择行！");
                return;
            }
            int index = dataGrid.CurrentRow.Index;
            if (index < 0)
            {
                MessageBox.Show("请选择行！");
                return;
            }
            SetInfo set = new SetInfo();
            set.title = dataGrid.Rows[index].Cells["title"].Value.ToString().Trim();
            set.time = dataGrid.Rows[index].Cells["time"].Value.ToString().Trim();
            set.rate = dataGrid.Rows[index].Cells["rate"].Value.ToString().Trim();
            set.interval = dataGrid.Rows[index].Cells["interval"].Value.ToString().Trim();
            set.memo = dataGrid.Rows[index].Cells["memo"].Value.ToString().Trim();
            set.url = dataGrid.Rows[index].Cells["url"].Value.ToString().Trim();
            set.enable = dataGrid.Rows[index].Cells["enable"].Value.ToString().Trim();
            AddorModifyForm frm = new AddorModifyForm(false, set);
            frm.ShowDialog();
            Refresh();
        }
        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dataGrid.DataSource == null)
            {
                MessageBox.Show("请选择行！");
                return;
            }
            int index = dataGrid.CurrentRow.Index;
            if (index < 0)
            {
                MessageBox.Show("请选择行！");
                return;
            }
            string title = dataGrid.Rows[index].Cells["title"].Value.ToString().Trim();
            OperationData.DelSetXml(title);
            Refresh();
        }
        #endregion

        #region  服务启动 Tap 页

        List<Thread> listThread = null;
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            List<SetInfo> setInfos = OperationData.GetSetData();
            listThread = new List<Thread>(setInfos.Count);//技术：多线程
            if (setInfos.Count == 0)
            {
                MessageBox.Show("没有相关配置！");
                return;
            }
            Thread thread = null;
            int i = 1;
            foreach (SetInfo info in setInfos)
            {
                i = 1;
                if (info.enable == "Y")
                {
                    thread = new Thread(() => { CheckWebUrl(info); });
                    thread.Name = info.title;
                    listThread.Add(thread);
                    i++;
                }
            }
            foreach (Thread tempThread in listThread)
            {
                if (tempThread.ThreadState == ThreadState.Unstarted)
                {
                    ThreadProcSafe(tempThread.Name + " 服务已开启!");
                    tempThread.Start();
                }
            }
        }
        /// <summary>
        /// 检查网站访问状态
        /// </summary>
        /// <param name="info"></param>
        private void CheckWebUrl(SetInfo info)
        {
            while (true)
            {
                if (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") == info.time)
                {
                    for (int i = 0; i < int.Parse(info.rate); i++)
                    {
                        RequestUrl request = new RequestUrl();
                        string mes = request.HttpGet(info);
                        ThreadProcSafe(mes);
                        Thread.Sleep(int.Parse(info.interval) * 1000);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            foreach (Thread tempThread in listThread)
            {
                if (tempThread.ThreadState != ThreadState.Aborted)
                {
                    ThreadProcSafe(tempThread.Name + " 服务已关闭!");
                    tempThread.Abort();
                }
            }
        }
        #endregion

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
