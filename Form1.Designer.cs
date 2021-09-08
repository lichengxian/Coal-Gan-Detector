namespace CoalMine
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.pbImage_A = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.text_result = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.comboBox_port = new System.Windows.Forms.ComboBox();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_info = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_client = new System.Windows.Forms.TextBox();
            this.textBox_time = new System.Windows.Forms.TextBox();
            this.textBox_index = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_count = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage_A)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(626, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(730, 40);
            this.label1.TabIndex = 6;
            this.label1.Text = "煤矸智能分拣系统 （v2020.10）";
            // 
            // pbImage_A
            // 
            this.pbImage_A.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage_A.Location = new System.Drawing.Point(139, 251);
            this.pbImage_A.Name = "pbImage_A";
            this.pbImage_A.Size = new System.Drawing.Size(861, 636);
            this.pbImage_A.TabIndex = 7;
            this.pbImage_A.TabStop = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(1478, 705);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "串口通信";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(134, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 30);
            this.label4.TabIndex = 11;
            this.label4.Text = "识别结果";
            // 
            // text_result
            // 
            this.text_result.Font = new System.Drawing.Font("宋体", 18F);
            this.text_result.Location = new System.Drawing.Point(275, 188);
            this.text_result.Name = "text_result";
            this.text_result.Size = new System.Drawing.Size(92, 35);
            this.text_result.TabIndex = 12;
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOpen.Font = new System.Drawing.Font("宋体", 14F);
            this.btnOpen.ForeColor = System.Drawing.Color.Navy;
            this.btnOpen.Location = new System.Drawing.Point(1180, 184);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(114, 51);
            this.btnOpen.TabIndex = 13;
            this.btnOpen.Text = "连接相机";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Font = new System.Drawing.Font("宋体", 14F);
            this.btnClose.ForeColor = System.Drawing.Color.Navy;
            this.btnClose.Location = new System.Drawing.Point(1181, 267);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(113, 51);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "关闭相机";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // comboBox_port
            // 
            this.comboBox_port.Font = new System.Drawing.Font("宋体", 15F);
            this.comboBox_port.FormattingEnabled = true;
            this.comboBox_port.ItemHeight = 20;
            this.comboBox_port.Location = new System.Drawing.Point(1623, 705);
            this.comboBox_port.Name = "comboBox_port";
            this.comboBox_port.Size = new System.Drawing.Size(266, 28);
            this.comboBox_port.TabIndex = 16;
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnOpenPort.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOpenPort.Font = new System.Drawing.Font("宋体", 14F);
            this.btnOpenPort.ForeColor = System.Drawing.Color.Navy;
            this.btnOpenPort.Location = new System.Drawing.Point(1781, 758);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(108, 42);
            this.btnOpenPort.TabIndex = 17;
            this.btnOpenPort.Text = "打开串口";
            this.btnOpenPort.UseVisualStyleBackColor = false;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox_Port.Location = new System.Drawing.Point(1483, 758);
            this.textBox_Port.Multiline = true;
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Port.Size = new System.Drawing.Size(277, 120);
            this.textBox_Port.TabIndex = 19;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSend.Font = new System.Drawing.Font("宋体", 14F);
            this.btnSend.ForeColor = System.Drawing.Color.Navy;
            this.btnSend.Location = new System.Drawing.Point(1781, 832);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(108, 46);
            this.btnSend.TabIndex = 20;
            this.btnSend.Text = "发送数据";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(1478, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 30);
            this.label7.TabIndex = 21;
            this.label7.Text = "网络通信";
            // 
            // textBox_info
            // 
            this.textBox_info.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox_info.Location = new System.Drawing.Point(1483, 275);
            this.textBox_info.Multiline = true;
            this.textBox_info.Name = "textBox_info";
            this.textBox_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_info.Size = new System.Drawing.Size(406, 386);
            this.textBox_info.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(1478, 225);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 30);
            this.label8.TabIndex = 23;
            this.label8.Text = "客户端";
            // 
            // textBox_client
            // 
            this.textBox_client.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox_client.Location = new System.Drawing.Point(1581, 221);
            this.textBox_client.Name = "textBox_client";
            this.textBox_client.Size = new System.Drawing.Size(188, 35);
            this.textBox_client.TabIndex = 24;
            // 
            // textBox_time
            // 
            this.textBox_time.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox_time.Location = new System.Drawing.Point(650, 188);
            this.textBox_time.Name = "textBox_time";
            this.textBox_time.Size = new System.Drawing.Size(136, 35);
            this.textBox_time.TabIndex = 25;
            // 
            // textBox_index
            // 
            this.textBox_index.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox_index.Location = new System.Drawing.Point(885, 188);
            this.textBox_index.Name = "textBox_index";
            this.textBox_index.Size = new System.Drawing.Size(115, 35);
            this.textBox_index.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(509, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 30);
            this.label2.TabIndex = 27;
            this.label2.Text = "运行时间";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(797, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 30);
            this.label5.TabIndex = 28;
            this.label5.Text = "坐标";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1781, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 39);
            this.button1.TabIndex = 29;
            this.button1.Text = "发送测试坐标 ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_count
            // 
            this.textBox_count.Font = new System.Drawing.Font("宋体", 16F);
            this.textBox_count.Location = new System.Drawing.Point(305, 898);
            this.textBox_count.Multiline = true;
            this.textBox_count.Name = "textBox_count";
            this.textBox_count.Size = new System.Drawing.Size(118, 35);
            this.textBox_count.TabIndex = 30;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 16F);
            this.label6.Location = new System.Drawing.Point(135, 907);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(164, 22);
            this.label6.TabIndex = 31;
            this.label6.Text = "当日煤块数量：";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 12F);
            this.button2.Location = new System.Drawing.Point(438, 898);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 34);
            this.button2.TabIndex = 32;
            this.button2.Text = "重置计数";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(3, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(1920, 5);
            this.label9.TabIndex = 33;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button3.ForeColor = System.Drawing.Color.Navy;
            this.button3.Location = new System.Drawing.Point(1063, 513);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(87, 54);
            this.button3.TabIndex = 34;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = System.Drawing.Color.Navy;
            this.button4.Location = new System.Drawing.Point(1063, 435);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(87, 55);
            this.button4.TabIndex = 35;
            this.button4.Text = "启动";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.ForeColor = System.Drawing.Color.Red;
            this.button5.Location = new System.Drawing.Point(1063, 592);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(87, 57);
            this.button5.TabIndex = 36;
            this.button5.Text = "急停";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Gray;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(1181, 391);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 386);
            this.label10.TabIndex = 37;
            this.label10.Text = "皮  带";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(1335, 548);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(87, 55);
            this.button6.TabIndex = 38;
            this.button6.Text = "启动";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.Location = new System.Drawing.Point(1335, 630);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(87, 51);
            this.button7.TabIndex = 39;
            this.button7.Text = "停止";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button8.ForeColor = System.Drawing.Color.Red;
            this.button8.Location = new System.Drawing.Point(1336, 713);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(86, 54);
            this.button8.TabIndex = 40;
            this.button8.Text = "急停";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(1060, 391);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 14);
            this.label11.TabIndex = 41;
            this.label11.Text = "机器人（南）";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(1331, 391);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 14);
            this.label12.TabIndex = 42;
            this.label12.Text = "机器人（北）";
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label13.Location = new System.Drawing.Point(1029, 109);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(5, 828);
            this.label13.TabIndex = 43;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label14.Location = new System.Drawing.Point(1446, 109);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(5, 828);
            this.label14.TabIndex = 44;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1488, 844);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_count);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_index);
            this.Controls.Add(this.textBox_time);
            this.Controls.Add(this.textBox_client);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox_info);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textBox_Port);
            this.Controls.Add(this.btnOpenPort);
            this.Controls.Add(this.comboBox_port);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.text_result);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pbImage_A);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = " ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage_A)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;                    //Label 1 “煤矸智能分拣系统”
        private System.Windows.Forms.PictureBox pbImage_A;            //PictureBox 图像
        private System.Windows.Forms.Label label3;                    //Label 3 “串口通信”
        private System.Windows.Forms.Label label4;                    //Label 4 “识别结果”
        private System.Windows.Forms.TextBox text_result;             //TextBox 识别结果
        private System.Windows.Forms.Button btnOpen;                  //Button 连接相机
        private System.Windows.Forms.Button btnClose;                 //Button 关闭相机
        private System.Windows.Forms.ComboBox comboBox_port;          //ComboBox 串口下拉框
        private System.Windows.Forms.Button btnOpenPort;              //Button 打开串口
        private System.Windows.Forms.TextBox textBox_Port;            //TextBox 串口信息
        private System.Windows.Forms.Button btnSend;                  //Button 发送数据
        private System.Windows.Forms.Label label7;                    //Label 7 “网络通信”
        private System.Windows.Forms.TextBox textBox_info;            //TextBox 网络信息框
        private System.Windows.Forms.Label label8;                    //Label 8 “客户端”
        private System.Windows.Forms.TextBox textBox_client;          //TextBox 客户端
        private System.Windows.Forms.TextBox textBox_time;            //TextBox 运行时间
        private System.Windows.Forms.TextBox textBox_index;           //TextBox 坐标
        private System.Windows.Forms.Label label2;                    //Label 2 “运行时间”
        private System.Windows.Forms.Label label5;                    //Label 5 “坐标”
        private System.Windows.Forms.Button button1;                  //Button 发送测试坐标
        private System.Windows.Forms.TextBox textBox_count;           //TextBox 煤块数量
        private System.Windows.Forms.Label label6;                    //Label 6 “当日煤块数量：”
        private System.Windows.Forms.Button button2;                  //Button 重置计数
        private System.Windows.Forms.Label label9;                    //Label 9 白色线
        private System.Windows.Forms.Button button3;                  //Button 停止（南）
        private System.Windows.Forms.Button button4;                  //Button 启动（南）
        private System.Windows.Forms.Button button5;                  //Button 急停（南）
        private System.Windows.Forms.Label label10;                   //Label 10 “皮带”
        private System.Windows.Forms.Button button6;                  //Button 启动（北）
        private System.Windows.Forms.Button button7;                  //Button 停止（北）
        private System.Windows.Forms.Button button8;                  //Button 急停（北）
        private System.Windows.Forms.Label label11;                   //Label 11 “机器人（南）”
        private System.Windows.Forms.Label label12;                   //Label 12 “机器人（北）”
        private System.Windows.Forms.Label label13;                   //Label 13 白色线
        private System.Windows.Forms.Label label14;                   //Label 14 白色线
    }
}

