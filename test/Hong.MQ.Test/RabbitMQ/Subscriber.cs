using System.Windows.Forms;

namespace Hong.MQ.Test
{
    public partial class Subscriber : Form
    {
        Subscribe subscribe = null;
        const string ip = "192.168.1.104";

        public Subscriber(string[] queueKey)
        {
            InitializeComponent();

            this.Text = "订阅者 ->接收队列:" + string.Join(",", queueKey) + " 的消息";

            subscribe = new Subscribe(ip, 5672, "zhanghong", "zhanghong@2016", true, new Subscribe.ReciveCallBack(CallBack));
            subscribe.Start(queueKey);
        }

        private void Subscribe_FormClosed(object sender, FormClosedEventArgs e)
        {
            subscribe.Close();
        }

        void CallBack(string queueKey, string msg)
        {
            this.Invoke(new Subscribe.ReciveCallBack(Show), new object[] { queueKey, msg });
        }

        void Show(string queueKey, string msg)
        {
            this.richTextBox1.AppendText("来源队列:" + queueKey + "-> " + msg);
            this.richTextBox1.AppendText("\r\n");
        }
    }
}
