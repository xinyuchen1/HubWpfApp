using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubWpfApp
{
    [HubName("noticeHub")]
    public class HubHepler : Hub
    {
        public void UpdateSchedule(string code, string model)
        {

        }

        public void updateSchedule(string code, string model)
        {

        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ScheduleViewModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 是否声音提醒
        /// </summary>
        public bool voice { get; set; }
        /// <summary>
        /// 图标名称
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 标题内容
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime starttime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endtime { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
}
