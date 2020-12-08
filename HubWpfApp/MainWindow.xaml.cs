using Common;
using Microsoft.AspNet.SignalR.Client;
using SmartCalendarTips.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HubWpfApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public IHubProxy HubProxy { get; set; }
        const string ServerUri = "https://localhost:44341/signalr";

        public HubConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 窗口启动时开始连接服务
            ConnectAsync(); 
        }

       

        public static void WriteLog(string value)
        {
            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true, Encoding.Default))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss:fff") + "\t" + value);
            }
        } 

        private async void ConnectAsync()
        {
            Connection = new HubConnection(ServerUri);
            Connection.StateChanged += Connection_StateChanged;
            Connection.Closed += Connection_Closed;

            // 创建一个集线器代理对象
            HubProxy = Connection.CreateHubProxy("noticeHub");

            // 供服务端调用，将消息输出到消息列表框中
            HubProxy.On<string, dynamic>("schemeBreakdown", (code, sendData) =>
                  this.Dispatcher.Invoke(() =>
                    {

                    }
                 )
                );

            HubProxy.On<string, dynamic>("schemeNew", (code, sendData) =>
                this.Dispatcher.Invoke(() =>
                {
               
                }
               )
              );

            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                // 连接失败
                return;
            }
             
        }

        private void Connection_StateChanged(StateChange obj)
        {
            if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                HubProxy.Invoke("Login", "SH20201123001");
        }

        private void Connection_Closed()
        {

        }
    } 
}
