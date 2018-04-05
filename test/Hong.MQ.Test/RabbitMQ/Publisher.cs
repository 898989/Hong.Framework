using System;
using System.Threading;
using System.Windows.Forms;

namespace Hong.MQ.Test
{
    public partial class Publisher : Form
    {
        Publish publish = null;
        const string ip = "192.168.1.104";

        public Publisher()
        {
            InitializeComponent();

            publish = new RabbitMQ.Publish(ip, 5672, "zhanghong", "zhanghong@2016");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            if (tabControl1.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(this.domainUpDown1.Text.Trim()))
                {
                    MessageBox.Show("请输入队列名称");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "接收者:" + this.domainUpDown1.Text.Trim() + "->内容:" + msg;
                publish.SendMsg(this.domainUpDown1.Text.Trim(), msg);
            }
            else
            {
                if (string.IsNullOrEmpty(this.t_exchange.Text.Trim()))
                {
                    MessageBox.Show("请输入交换机");
                    return;
                }

                if (string.IsNullOrEmpty(this.t_routkey.Text.Trim()))
                {
                    MessageBox.Show("请输入路由标识");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "发送给:" + this.t_exchange.Text.Trim() + "->消息线路" + this.t_routkey.Text.Trim() + "->内容:" + msg;

                publish.SendMsg(this.t_exchange.Text.Trim(), this.t_routkey.Text.Trim(), msg);
            }

            this.richTextBox1.AppendText(msg);
            this.richTextBox1.AppendText("\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string channel = "channel_" + new Random().Next(2000);

            Manager m = new Manager(ip, 5672, "zhanghong", "zhanghong@2016");
            m.CreateQueue(channel, true);

            this.domainUpDown1.Items.Add(channel);
            this.domainUpDown1.Text = channel;

            m.Close();
        }

        private void Publish_FormClosed(object sender, FormClosedEventArgs e)
        {
            publish.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.domainUpDown1.Text))
            {
                MessageBox.Show("请输入队列标识");
                return;
            }

            Subscribe s = new Subscribe(new string[] { this.domainUpDown1.Text });
            s.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            if (tabControl1.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(this.domainUpDown1.Text.Trim()))
                {
                    MessageBox.Show("请输入队列名称");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "接收者:" + this.domainUpDown1.Text.Trim() + "->内容:" + msg;

                for (int i = 0; i < 1000; i++)
                    publish.SendMsg(this.domainUpDown1.Text.Trim(), msg);
            }
            else
            {
                if (string.IsNullOrEmpty(this.t_exchange.Text.Trim()))
                {
                    MessageBox.Show("请输入交换机");
                    return;
                }

                if (string.IsNullOrEmpty(this.t_routkey.Text.Trim()))
                {
                    MessageBox.Show("请输入路由标识");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "发送给:" + this.t_exchange.Text.Trim() + "->消息线路" + this.t_routkey.Text.Trim() + "->内容:" + msg;

                for (int i = 0; i < 1000; i++)
                    publish.SendMsg(this.t_exchange.Text.Trim(), this.t_routkey.Text.Trim(), msg);
            }

            this.richTextBox1.AppendText(msg + "(连续发送1000次)");
            this.richTextBox1.AppendText("\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Subscribe s = new Subscribe(this.t_queue.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            s.Show();
        }

        void Send(object parma)
        {
            ThreadParams p = (ThreadParams)parma;

            for (int i = 0; i < 250; i++)
            {
                publish.SendMsg(p.Queue, p.Msg + "." + i);
            }
        }

        void Send1(object parma)
        {
            ThreadParams p = (ThreadParams)parma;

            for (int i = 0; i < 250; i++)
            {
                publish.SendMsg(p.Exchange, p.Routkey, p.Msg + "." + i);
            }
        }

        class ThreadParams
        {
            public string Exchange;
            public string Routkey;
            public string Queue;
            public string Msg;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread x1, x2, x3, x4;
            ThreadParams para;
            string msg = string.Empty;

            if (tabControl1.SelectedIndex == 0)
            {
                if (string.IsNullOrEmpty(this.domainUpDown1.Text.Trim()))
                {
                    MessageBox.Show("请输入队列名称");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "接收者:" + this.domainUpDown1.Text.Trim() + "->内容:" + msg;

                x1 = new Thread(new System.Threading.ParameterizedThreadStart(Send));
                x2 = new Thread(new System.Threading.ParameterizedThreadStart(Send));
                x3 = new Thread(new System.Threading.ParameterizedThreadStart(Send));
                x4 = new Thread(new System.Threading.ParameterizedThreadStart(Send));

                para = new ThreadParams()
                {
                    Queue = this.domainUpDown1.Text.Trim(),
                    Msg = msg
                };
            }
            else
            {
                if (string.IsNullOrEmpty(this.t_exchange.Text.Trim()))
                {
                    MessageBox.Show("请输入交换机");
                    return;
                }

                if (string.IsNullOrEmpty(this.t_routkey.Text.Trim()))
                {
                    MessageBox.Show("请输入路由标识");
                    return;
                }

                msg = this.textBox2.Text.Trim();
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("请输入发送的消息");
                    return;
                }

                msg = "发送给:" + this.t_exchange.Text.Trim() + "->消息线路" + this.t_routkey.Text.Trim() + "->内容:" + msg;

                x1 = new Thread(new System.Threading.ParameterizedThreadStart(Send1));
                x2 = new Thread(new System.Threading.ParameterizedThreadStart(Send1));
                x3 = new Thread(new System.Threading.ParameterizedThreadStart(Send1));
                x4 = new Thread(new System.Threading.ParameterizedThreadStart(Send1));

                para = new ThreadParams()
                {
                    Exchange = this.t_exchange.Text.Trim(),
                    Routkey = this.t_routkey.Text.Trim(),
                    Msg = msg
                };
            }

            x1.IsBackground = true;
            x2.IsBackground = true;
            x3.IsBackground = true;
            x4.IsBackground = true;
            x1.Start(para);
            x2.Start(para);
            x3.Start(para);
            x4.Start(para);

            this.richTextBox1.AppendText(msg + "(连续发送1000次)");
            this.richTextBox1.AppendText("\r\n");
        }
    }
}
