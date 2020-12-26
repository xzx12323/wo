using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeWorkDemo
{
    /// <summary>
    /// 新增或修改窗体
    /// </summary>
    public partial class AddorModifyForm : Form
    {
        private bool _isAdd = false;
        private SetInfo info { get; set; }
        public AddorModifyForm(bool IsAdd, SetInfo Info = null)
        {
            InitializeComponent();
            this._isAdd = IsAdd;
            this.info = Info;
            init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            
            if (!_isAdd)
            {
                txtTitle.Enabled = false;
                txtTitle.Text = info.title;
                txtDate.Value = DateTime.Parse(info.time);
                txtRate.Text = info.rate;
                txtInterval.Text = info.interval;
                txtMemo.Text = info.memo;
                txtUrl.Text = info.url;
                checkEnable.Checked = info.enable == "Y" ? true : false;
            }
            else
            {
                txtTitle.Enabled = true;
            }

        }

        #region 事件
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 校验
            if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
            {
                MessageBox.Show("标题不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(txtDate.Text.Trim()))
            {
                MessageBox.Show("秒杀时间不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                MessageBox.Show("秒杀频率不能为空！");
                return;
            }
            else
            {
                try
                {
                    int a = int.Parse(txtRate.Text.Trim());
                    if (a <= 0)
                    {
                        MessageBox.Show("秒杀频率必须大于0！");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("秒杀频率必须为数字！");
                    return;
                }
            }
            if (string.IsNullOrEmpty(txtInterval.Text.Trim()))
            {
                MessageBox.Show("时间间隔不能为空！");
                return;
            }
            else
            {
                try
                {
                    int b = int.Parse(txtInterval.Text.Trim());
                    if (b <= 0)
                    {
                        MessageBox.Show("时间间隔必须大于0！");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("时间间隔必须为数字！");
                    return;
                }
            }
            if (string.IsNullOrEmpty(txtUrl.Text.Trim()))
            {
                MessageBox.Show("秒杀链接不能为空！");
                return;
            }
            #endregion

            SetInfo set = new SetInfo();
            set.title = txtTitle.Text;
            set.time = txtDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            set.rate = txtRate.Text;
            set.interval = txtInterval.Text;
            set.memo = txtMemo.Text;
            set.url = txtUrl.Text;
            set.enable = checkEnable.Checked == true ? "Y" : "N";//三元表达示

            if (_isAdd)//新增
            {
                OperationData.AddSetXml(set);
                this.Close();
            }
            else//修改
            {
                OperationData.UpdateSetXml(set);
                this.Close();
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtTitle.Text = string.Empty;
            txtDate.Text = string.Empty;
            txtRate.Text = string.Empty;
            txtInterval.Text = string.Empty;
            txtMemo.Text = string.Empty;
            txtUrl.Text = string.Empty;
            checkEnable.Checked = false;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
