namespace Hong.MQ.Test
{
    partial class Publisher
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.sendto_channel = new System.Windows.Forms.TabPage();
            this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.sendto_exchange = new System.Windows.Forms.TabPage();
            this.t_exchange = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.t_queue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.t_routkey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.sendto_channel.SuspendLayout();
            this.sendto_exchange.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 419F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(935, 716);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 300);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(929, 413);
            this.panel1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Location = new System.Drawing.Point(12, 275);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(898, 117);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "发送消息";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "消息:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(77, 33);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(767, 25);
            this.textBox2.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Location = new System.Drawing.Point(78, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button6
            // 
            this.button6.AutoSize = true;
            this.button6.Location = new System.Drawing.Point(338, 78);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(169, 25);
            this.button6.TabIndex = 16;
            this.button6.Text = "四线程并发发布1000次";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.AutoSize = true;
            this.button5.Location = new System.Drawing.Point(183, 78);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(124, 25);
            this.button5.TabIndex = 7;
            this.button5.Text = "连续发送1000次";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.sendto_channel);
            this.tabControl1.Controls.Add(this.sendto_exchange);
            this.tabControl1.Location = new System.Drawing.Point(12, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(902, 244);
            this.tabControl1.TabIndex = 17;
            // 
            // sendto_channel
            // 
            this.sendto_channel.Controls.Add(this.domainUpDown1);
            this.sendto_channel.Controls.Add(this.button3);
            this.sendto_channel.Controls.Add(this.button4);
            this.sendto_channel.Controls.Add(this.label3);
            this.sendto_channel.Location = new System.Drawing.Point(4, 25);
            this.sendto_channel.Name = "sendto_channel";
            this.sendto_channel.Padding = new System.Windows.Forms.Padding(3);
            this.sendto_channel.Size = new System.Drawing.Size(894, 215);
            this.sendto_channel.TabIndex = 0;
            this.sendto_channel.Text = "消息发送到通道";
            this.sendto_channel.UseVisualStyleBackColor = true;
            // 
            // domainUpDown1
            // 
            this.domainUpDown1.Location = new System.Drawing.Point(155, 45);
            this.domainUpDown1.Name = "domainUpDown1";
            this.domainUpDown1.Size = new System.Drawing.Size(144, 25);
            this.domainUpDown1.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.Location = new System.Drawing.Point(155, 90);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(122, 25);
            this.button3.TabIndex = 4;
            this.button3.Text = "创建输入的队列";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.AutoSize = true;
            this.button4.Location = new System.Drawing.Point(155, 132);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(152, 25);
            this.button4.TabIndex = 6;
            this.button4.Text = "新增该队列的订阅者";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "输入通道名称:";
            // 
            // sendto_exchange
            // 
            this.sendto_exchange.Controls.Add(this.t_exchange);
            this.sendto_exchange.Controls.Add(this.label1);
            this.sendto_exchange.Controls.Add(this.button2);
            this.sendto_exchange.Controls.Add(this.t_queue);
            this.sendto_exchange.Controls.Add(this.label4);
            this.sendto_exchange.Controls.Add(this.t_routkey);
            this.sendto_exchange.Controls.Add(this.label2);
            this.sendto_exchange.Location = new System.Drawing.Point(4, 25);
            this.sendto_exchange.Name = "sendto_exchange";
            this.sendto_exchange.Padding = new System.Windows.Forms.Padding(3);
            this.sendto_exchange.Size = new System.Drawing.Size(894, 215);
            this.sendto_exchange.TabIndex = 1;
            this.sendto_exchange.Text = "消息发送给交换机";
            this.sendto_exchange.UseVisualStyleBackColor = true;
            // 
            // t_exchange
            // 
            this.t_exchange.Location = new System.Drawing.Point(242, 30);
            this.t_exchange.Name = "t_exchange";
            this.t_exchange.Size = new System.Drawing.Size(196, 25);
            this.t_exchange.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "输入消息发磅到的交换机名称:";
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(718, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 25);
            this.button2.TabIndex = 13;
            this.button2.Text = "新增该队列的订阅者";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // t_queue
            // 
            this.t_queue.Location = new System.Drawing.Point(242, 155);
            this.t_queue.Name = "t_queue";
            this.t_queue.Size = new System.Drawing.Size(466, 25);
            this.t_queue.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 15);
            this.label4.TabIndex = 14;
            this.label4.Text = "队列,多个用逗号分隔:";
            // 
            // t_routkey
            // 
            this.t_routkey.Location = new System.Drawing.Point(242, 78);
            this.t_routkey.Name = "t_routkey";
            this.t_routkey.Size = new System.Drawing.Size(196, 25);
            this.t_routkey.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "输入消息经过的路由标识:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(929, 291);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // Publish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 716);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Publish";
            this.Text = "发布者";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Publish_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.sendto_channel.ResumeLayout(false);
            this.sendto_channel.PerformLayout();
            this.sendto_exchange.ResumeLayout(false);
            this.sendto_exchange.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DomainUpDown domainUpDown1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox t_exchange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox t_routkey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox t_queue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage sendto_channel;
        private System.Windows.Forms.TabPage sendto_exchange;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
    }
}

