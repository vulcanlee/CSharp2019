using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoAwait
{
    public partial class Form1 : Form
    {
        string result;
        int sleepms = 2000;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnImput_Click(object sender, EventArgs e)
        {

        }
        async Task AutoInputAsync()
        {
            Message.Text = "準備輸入資料";
            await Task.Delay(sleepms);
            AddValue1.Text = "231";
            await Task.Delay(sleepms);
            AddValue2.Text = "89";
            Message.Text = "資料輸入完成";
            await Task.Delay(sleepms);
        }
        private async void btnCallWebAPI_Click(object sender, EventArgs e)
        {
            await CallWebAPIAsync();
        }
        async Task CallWebAPIAsync()
        {
            Message.Text = "呼叫 Web API 進行加總計算";
            await Task.Delay(sleepms);
            string host = "https://lobworkshop.azurewebsites.net";
            string path = $"/api/RemoteSource/Add/{AddValue1.Text}/{AddValue2.Text}/5";
            string url = $"{host}{path}";
            progressBar1.Style = ProgressBarStyle.Marquee;
            result = await new HttpClient().GetStringAsync(url);
            Message.Text = "已經取得計算結果";
            await Task.Delay(sleepms);
            AddValueSum.Text = result;
        }
        private async void btnStop_Click(object sender, EventArgs e)
        {
            await StopAsync();
        }
        async Task StopAsync()
        {
            Message.Text = "停止自動操作";
            progressBar1.Style = ProgressBarStyle.Blocks;
            await Task.Delay(sleepms);
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Blocks;
            btnImput.Focus();
            await AutoInputAsync();
            btnCallWebAPI.Focus();
            await CallWebAPIAsync();
            btnStop.Focus();
            await StopAsync();
            this.Focus();
        }
    }
}
