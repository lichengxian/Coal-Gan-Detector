using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThridLibray;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DllCoalGanDetect;
using Darknet;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Drawing.Imaging;
//using Basler.Pylon;

namespace CoalMine
{
    public partial class Form1 : Form
    {
        private static byte[] result = new byte[4096];

        /* ---创建socket连接机器人--- */
        public static Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //client socket
        //static Socket ClientSocket;
        //robots ip list
        string []robots_ips = {"192.168.0.5", "192.168.0.6"}; //多个机器人
        uint counter = 0; //用于多个机器人
        //ip socket tabels
        Hashtable ip_socket_tb = null;

        /* ---创建串口设备对象--- */
        public CommClass _serial;

        /* ---创建摄像头--- */
        private IDevice m_dev_A;       //OPT摄像头
        string ipA = "192.168.0.11";

        /* ---软件定时器timer--- */
        public static System.Timers.Timer picture_trigger; // 按照给定时间自动定时触发
        //public static System.Timers.Timer onetime_trigger; // 在程序中触发，非自动触发
        int trigger_start;
        //trigger mode
        private bool is_software_trigger = true;

        //存放坐标位置
        //int coor_x;
        //int coor_y;
        //int flag = 0;

        /* ---锁，保证多线程安全--- */
        Mutex m_mutex = new Mutex();

        /* ---各种保存目录--- */
        //图片保存地址
        string sourcepath_A = @"D:\Coal\";
        //预测图片结果保存
        string pred_results_root = @"D:\pred_results\";
        //日志文件地址
        static private string log_dir = @"..\..\..\log\";
        //cfg file
        //static private string cfg_file = @"..\..\..\cfgs\cfg.ini";   
        //log file for gaotong detector --- not implemented yet
        //static private string gaotong_log = @"..\..\..\log\gaotong.log";
        //log file for GUI test
        private System.IO.StreamWriter logfile = null;
        private System.IO.StreamWriter error_log_file = null;

        /* ---建立数据库--- */
        static string connstr = "data source=localhost;database=coal;user id=root;password=zkhy2020;pooling=false;charset=utf8";
        MySqlConnection conn = new MySqlConnection(connstr);
        //string nowaday = DateTime.Now.ToString("yyyy-MM-dd");
        //int result = string.Compare(nowaday, lastday);//日切

        /* ---YOLO detector--- */
        //private DllCoalGanDetect.CoalGanDetector gaotong_detector = null;
        private YoloWrapper yolo_detector = null;
        static private float score_thresh = 0.1f;  //检测阈值

        /* ---image window size and image size--- */
        private int im_win_w_;
        private int im_win_h_;
        private int im_w_;
        private int im_h_;
        private float scale_x;
        private float scale_y;

        /* ---检测图像是否静止所用变量--- */
        private int g_iRegSum;
        private Point pPoint;

        public Form1()
        {
            InitializeComponent();       //初始化控件
            CheckForIllegalCrossThreadCalls = false;
            _serial = new CommClass();
            g_iRegSum = 0;
            pPoint = new Point(0, 0);

            /* ---配置日志文件--- */
            log_dir = create_dir(log_dir);
            logfile = new System.IO.StreamWriter(Path.Combine(log_dir, "时间测试记录.txt"));
            logfile.AutoFlush = true;

            /* ---错误日志文件--- */
            error_log_file = new System.IO.StreamWriter(Path.Combine(log_dir, "error_log.txt"), true);
            error_log_file.AutoFlush = true;
            error_log_file.WriteLine("===============================");
            error_log_file.WriteLine("start error logger @{0}",DateTime.Now);
            error_log_file.WriteLine("===============================");
            logfile.AutoFlush = true;
            logfile.WriteLine("===============================");
            logfile.WriteLine("start logger @{0}", DateTime.Now);
            logfile.WriteLine("===============================");

            /* ---初始化ip socket hash表--- */
            ip_socket_tb = new Hashtable();

            /* ---摄像头Bitmap保存在本地--- */
            create_dir(sourcepath_A);

            /* ---初始化detector--- */
            init_detector();

            /* ---用于测试detector--- */
            string img_path = @"..\..\..\tmp\test10.jpg";
            test_detector(img_path);
            //初始化禁止机器人控制按钮
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;

            /* ---初始化timer--- */
            if (is_software_trigger)
            {
                int trigger_interval = 2000; //2秒,防止一个目标多次拍摄
                picture_trigger = new System.Timers.Timer(trigger_interval); //设置时间间隔
                picture_trigger.Elapsed += new System.Timers.ElapsedEventHandler(software_trigger);
                picture_trigger.AutoReset = false; // true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）

                //onetime_trigger = new System.Timers.Timer(625);// (1250); //设置时间间隔
                //onetime_trigger.Elapsed += new System.Timers.ElapsedEventHandler(software_trigger);
                //onetime_trigger.AutoReset = false;
            }

            /* ---进行服务器端连接设置--- */
            try
            {
                int Port = 50123;
                IPAddress IP = IPAddress.Parse("192.168.0.206");//,192.168.0.206  //本机地址
                ServerSocket.Bind(new IPEndPoint(IP, Port));
                ServerSocket.Listen(3);
                //写入界面
                string text = "启动监听";
                text += ServerSocket.LocalEndPoint.ToString();                
                text += "\r\n";
                textBox_info.AppendText(text);
                textBox_info.ScrollToCaret();
                Thread thread = new Thread(ListenClientConnect);
                thread.Start();
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                error_log_file.WriteLine("Error: init server socket error");
                throw ex;
            }            
        }

        /* ---连接监听--- */
        public void ListenClientConnect()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            while (true)
            {
                Socket clnt_sock = ServerSocket.Accept();
                string remote_sock_ip = ((IPEndPoint)clnt_sock.RemoteEndPoint).Address.ToString();

                if(ip_socket_tb.Contains(remote_sock_ip))
                {
                    ip_socket_tb.Remove(remote_sock_ip);
                }
                ip_socket_tb.Add(remote_sock_ip, clnt_sock);
                this.Invoke(new Action(() =>
                {
                    //update gui
                    textBox_client.AppendText(remote_sock_ip+"; ");
                    //ClientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello!"));
                    textBox_info.AppendText(string.Format("client@{0} online\r\n", remote_sock_ip));
                    textBox_info.ScrollToCaret();
                    //使能机器人控制按钮
                    if (string.Equals(remote_sock_ip, robots_ips[0]))
                    {
                        button6.Enabled = true;
                        button7.Enabled = true;
                        button8.Enabled = true;
                    }
                    if (string.Equals(remote_sock_ip, robots_ips[1]))
                    {
                        button3.Enabled = true;
                        button4.Enabled = true;
                        button5.Enabled = true;
                    }
                }));                
                //break;
            }
        }

        /* ---创建路径--- */
        public string create_dir(string path)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
            else
            {
                Directory.CreateDirectory(path);
                return path;
            }
        }

        /* ---封装坐标发送函数--- */
        public void SendPoint(int coor_x, int coor_y, uint iRobot_num)
        {
            string clnt_ip = null;
            string send = null;
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                           System.Globalization.CultureInfo.InvariantCulture);
                logfile.WriteLine("Sent: Robot {3}: x {0}; y {1}, @ time:{2} ", coor_x, coor_y, timestamp, iRobot_num);
                //(-1,-1)坐标表示机器人停止
                if ((coor_x==-1) && (coor_y==-1))
                {
                    send = "STOP";
                }
                //(0,-1)坐标表示机器人启动
                else if ((coor_x == 0) && (coor_y == -1))
                {
                    send = "START";
                }
                //(-1,0)坐标表示机器人紧急停止
                else if ((coor_x == -1) && (coor_y == 0))
                {
                    send = "ESTOP";
                }
                //其他坐标正常发给机器人
                else
                {
                    send = coor_x.ToString() + " ," + coor_y.ToString();
                }

                //向客户端发送消息
                //ClientSocket.Send(Encoding.ASCII.GetBytes(send));

                //两个机器人情况
                //int clnt_index = (int)(gan_counter++) % robots_ips.Length;
                //clnt_ip = robots_ips[clnt_index];

                clnt_ip = robots_ips[iRobot_num];
                Socket clnt_sock = (Socket)ip_socket_tb[clnt_ip];
                clnt_sock.Send(Encoding.ASCII.GetBytes(send));

                Console.WriteLine("$$$ sent $$$");

                this.Invoke(new Action(() =>
                {
                    //update gui
                    textBox_info.AppendText(string.Format("send to client@{0}: {1}\r\n", clnt_ip, send));
                    textBox_info.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {
                error_log_file.WriteLine("Error: send point to client@{0} failed: {1}", clnt_ip, ex.ToString());
                //MyClientSocket.Shutdown(SocketShutdown.Both);
                //MyClientSocket.Close();
                //throw ex;
            }
        }

        /* ---引用外部的dll文件--- */
        [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
        class CPPDLL
        {
            //coal_rec.dll
            [DllImport("Gaotong.dll", EntryPoint = "Coal_Detection", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern int Coal_Detection(string testImgPath, double ratio, int threshold, ref CvPoint p);
        }

        #region YOLO detector
        /* ---初始化YOLO detector--- */
        private int init_detector()
        {
            //加载模型权重与配置文件
            string weights_file = @"..\..\..\model\darknet\yolov4-tiny\\coal_gan_20201212.weights";
            string cfg_file = @"..\..\..\model\darknet\yolov4-tiny\\coal_gan_20201212.cfg";

            yolo_detector = new Darknet.YoloWrapper(cfg_file, weights_file, 0);

            //init gaotong detector
            /*
            unsafe
            {
                IntPtr cfg_file_int_ptr = (IntPtr)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(cfg_file);
                sbyte* cfg_file_sbyte = (sbyte*)cfg_file_int_ptr;
                IntPtr gaotong_log_int_ptr = (IntPtr)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(gaotong_log);
                sbyte* gaotong_log_sbyte = (sbyte*)gaotong_log_int_ptr;
                gaotong_detector = new DllCoalGanDetect.CoalGanDetector(cfg_file_sbyte, gaotong_log_sbyte);
            }
            */
            Console.WriteLine("init YOLO detectors ok");
            return 0;
        }

        /* ---在NMS中计算IOU--- */
        private double iou(Darknet.YoloWrapper.bbox_t box1, Darknet.YoloWrapper.bbox_t box2)
        {
            double left = Math.Max(box1.x, box2.x);
            double x12 = box1.x + box1.w;
            double x22 = box2.x + box2.w;
            double right = Math.Min(x12, x22);
            double dx = right - left;
            dx = dx > 0 ? dx : 0;

            double top = Math.Max(box1.y, box2.y);
            double y12 = box1.y + box1.h;
            double y22 = box2.y + box2.h;
            double bottom = Math.Min(y12, y22);

            double dy = bottom - top;
            dy = dy > 0 ? dy : 0;
            
            return dx*dy / (box1.w * box1.h + box2.w * box2.h - dx*dy);
        }

        /* ---在coal-gan-detector中计算IOU--- */
        public int iou(int[] box1, int[] box2)
        {
            int xi1 = Math.Max(box1[0], box2[0]);
            int yi1 = Math.Max(box1[1], box2[1]);
            int xi2 = Math.Min(box1[2], box2[2]);
            int yi2 = Math.Min(box1[3], box2[3]);
            //int inter_area = (yi2 - yi1) * (xi2 - xi1);

            //int box1_area = (box1[2] - box1[0]) * (box1[3] - box1[1]);
            //int box2_area = (box2[2] - box2[0]) * (box2[3] - box2[1]);
            //int union_area = box1_area + box2_area - inter_area;
            if ((xi2 - xi1) > 0 && (yi2 - yi1) > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /* ---YOLO-NMS--- */
        private Darknet.YoloWrapper.bbox_t[] nms(Darknet.YoloWrapper.bbox_t[] boxes, double nms_thresh)
        {
            List<Darknet.YoloWrapper.bbox_t> ret = new List<Darknet.YoloWrapper.bbox_t>();
            // 默认darknet 返回的box已经排好序了，这里不再对box进行排序
            //int keep_count = 1;
            List<Darknet.YoloWrapper.bbox_t> mList = new List<Darknet.YoloWrapper.bbox_t>();
            mList.AddRange(boxes);
            while(mList.Count != 0)
            {
                ret.Add(mList[0]);
                for(int i = mList.Count-1; i > 0; i--)
                {
                    if(iou(mList[0], mList[i]) > nms_thresh)
                    {
                        mList.Remove(mList[i]);
                    }
                }
                mList.RemoveAt(0);
            }
            return ret.ToArray();
        }

        /* ---调节抓取的长宽裕度--- */
        public struct Marginn
        {
            public static int w = 205;
            public static int h = 0;
        }

        /* ---在检测物体上画框--- */
        public void draw_detections(Darknet.YoloWrapper.bbox_t[] ret)
        {
            if (_gA != null)
            {
                this.Invoke(new Action(() => {
                    int count = 0;
                    for (int i = 0; i < ret.Length; i++)
                    {
                        if (ret[i].prob < 0.0000001) break;
                        int rect_x = (int)(ret[i].x * scale_x);
                        int rect_y = (int)(ret[i].y * scale_y);

                        int rect_w = (int)(ret[i].w * scale_x);
                        int rect_h = (int)(ret[i].h * scale_y);
                        rect_w = rect_x + rect_w > im_win_w_ ? im_win_w_ - rect_x : rect_w;
                        rect_h = rect_y + rect_h > im_win_h_ ? im_win_h_ - rect_y : rect_h;

                        if (ret[i].obj_id == 1) //煤
                        {
                            _gA.DrawRectangle(new Pen(Color.Gold), rect_x, rect_y, rect_w, rect_h);
                        }
                        else
                        {
                            _gA.DrawRectangle(new Pen(Color.Silver), rect_x, rect_y, rect_w, rect_h);
                        }
                        count++;
                    }
                    Console.WriteLine("--------------------------------  detections: {0}", count);
                }));
            }
        }

        /* ---YOLO检测结果信息--- */
        public struct CvPoint
        {
            public int x;
            public int y;
            public int w;
            public int h;
        }

        /* ---煤矿目标检测--- */
        public int coal_gan_detect(string img_path, out CvPoint center) //, out CvPoint center_orig)
        {
            Darknet.YoloWrapper.bbox_t[] ret = yolo_detector.Detect(img_path); //, score_thresh);
            ret = nms(ret, 0.25);

            float max_score = -100;
            //float max_score_orig = -100;
            int max_idx = 0;
            //int max_idx_orig = 0;
            int iRsl = -1;
            int count_coal = 0;

            center.x = 0;
            center.y = 0;
            center.w = 0;
            center.h = 0;

            // 在检测物体上画框
            draw_detections(ret);

            // 判断皮带是否停止
            /*
            int i_x = (int)ret[0].x;
            int i_y = (int)ret[0].y;
            if (System.Math.Abs(i_x - pPoint.X)<20 && System.Math.Abs(i_y - pPoint.Y)<20)
            {
                iRsl = -1;
                return iRsl;
            }
            else
            {
                pPoint.X = i_x;
                pPoint.Y = i_y;
            }
            */
            for (int i = 0; i < ret.Length; i++)
            {
                //不在区域内
                if ((ret[i].prob > score_thresh) && ( ((ret[i].x - Marginn.w) <= 330) || ((ret[i].x + ret[i].w + Marginn.w) >= 2000)))  
                {
                    iRsl = -2;
                }
                //在区域内
                else if ((ret[i].prob > score_thresh) && ((ret[i].x - Marginn.w)>330) && ((ret[i].x + ret[i].w + Marginn.w) < 2000) && (ret[i].y > 160) && (ret[i].y < 1040)) //(ret[i].y>50) && (ret[i].y<1500)) //  else if ((ret[i].prob > score_thresh) && ((ret[i].x - Marginn.w)>330) && ((ret[i].x + ret[i].w + Marginn.w) < 1600) && (ret[i].y>180) && (ret[i].y<900) )390,1465是皮带两边的像素位置,防止抓手超出皮带而设置
                {
                    Console.WriteLine("num: {0}, prob: ({1}), obj: {2}, xywh:({3},{4},{5},{6})", i, ret[i].prob, ret[i].obj_id, ret[i].x, ret[i].y, ret[i].w, ret[i].h);

                    int flag = 0;

                    int[] box1 = { 0, 0, 0, 0 };
                    int[] box2 = { 0, 0, 0, 0 };

                    int iLenGrab = 330; //机器人夹具长度(像素单位)
                    if (ret[i].h < iLenGrab) 
                    {
                        Marginn.h = (int)((iLenGrab - ret[i].h) / 2);
                    }

                    // 考虑机器人抓手抓取空间
                    box1[0] = (int)(ret[i].x - Marginn.w);
                    box1[0] = (box1[0] > 0) ? box1[0] : 0;
                    box1[1] = (int)(ret[i].y - Marginn.h);
                    box1[1] = (box1[1]>0)? box1[1]:0;                    
                    box1[2] = (int)(box1[0] + ret[i].w + 2*Marginn.w);
                    box1[3] = (int)(box1[1] + ret[i].h + 2*Marginn.h);
                    
                    for (int j = 0; j < ret.Length; j++)
                    {
                        if (ret[j].prob > 0.1)
                        {
                            box2[0] = (int)(ret[j].x);
                            box2[1] = (int)(ret[j].y);
                            box2[2] = (int)(ret[j].x + ret[j].w);
                            box2[3] = (int)(ret[j].y + ret[j].h);
                            //如果其他框与加了裕量的框有交集则flag=1
                            if (i != j)
                                if (iou(box1, box2) == 1)
                                {
                                    Console.WriteLine("IOU exceed thresh: num:{0}, num:({1})", i, j);
                                    flag = 1;
                                    iRsl = -2;
                                    break;
                                }
                        }
                    }

                    if (ret[i].prob > score_thresh && ret[i].prob > max_score && flag == 0 && ret[i].obj_id >= 1)  //如果只抓煤，ret[i].obj_id == 1；否则只抓矸，ret[i].obj_id == 0
                    {
                        //煤宽度太大略过抓下一个合适的
                        if (ret[i].w > 300)
                        {
                            continue;
                        }
                        else
                        {
                            max_score = ret[i].prob;
                            max_idx = i;
                        }
                    }
                }

                /*
                else if (ret[i].prob > score_thresh && ret[i].prob > max_score_orig && ret[i].obj_id >= 0)
                {
                    max_score_orig = ret[i].prob;
                    max_idx_orig = i;
                }
                */

               // 煤块计数(与抓取和过滤无关)
	           if (ret[i].prob > score_thresh && ret[i].obj_id >= 1)
		           count_coal = count_coal + 1;
            }

            /*------- 链接数据库，更新煤块数据 --------*/
            if (count_coal > 0)
            {
                Console.WriteLine("open data");
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                string nowaday = DateTime.Now.ToString("yyyy-MM-dd");
                string sql_number = "SELECT number FROM coal_count WHERE DATE_FORMAT(time,'%Y-%m-%d') = DATE_FORMAT(NOW(), '%Y-%m-%d')";

                int coal_number = 0;
                int date_flag = 0;
                MySqlDataReader reader = null;
                try
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand(sql_number, conn);
                    reader = command.ExecuteReader();

                    /*while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            Console.WriteLine("read success");
                            coal_number = reader.GetInt32(0) + count_coal;
                        }
                        else
                        {
                            Console.WriteLine("Date changed");
                        }
                    }*/
                    if (reader.HasRows)
                    {
                        reader.Read();
                        Console.WriteLine("read success");
                        coal_number = reader.GetInt32(0) + count_coal;
                    }
                    else
                    {
                        Console.WriteLine("Date changed");
                        date_flag = 1;
                        coal_number = count_coal;
                    }
                    conn.Close();

                    if (date_flag == 1)
                    {
                        conn.Open();
                        string add_date = "insert into coal_count (time,number) VALUES ('" + nowaday + "', '" + coal_number + "')";
                        MySqlCommand cmd = new MySqlCommand(add_date, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        date_flag = 0;
                    }
                    else
                    {
                        conn.Open();
                        string sql_update = "update coal_count set number ='" + coal_number + "' where time = '" + nowaday + "'";
                        MySqlCommand cmd = new MySqlCommand(sql_update, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
                // 更新GUI
                this.textBox_count.Text = coal_number.ToString();

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                Console.WriteLine("close data");
            }
            /*-----------------------------------*/

            //有煤，但是不在区域内或机器人夹具距离不够
            if ((max_score == -100) && (iRsl == -2) && (count_coal>0)) 
            {                         
                return -3;                
            }
            //没有煤，有矸，但是不在区域内
            else if ((max_score == -100) && (iRsl == -2)) 
            {
                return -2;
            }
            //没有检测到物体
            else if (max_score == -100) 
            {
                error_log_file.WriteLine("Note: {0}: no object detected by yolo return -1 by default", img_path);
                return -1;
            }
            //检测到物体，并在区域内，返回检测信息
            else
            {
                center.x = (int)(ret[max_idx].x + 0.5 * ret[max_idx].w);
                center.y = (int)(ret[max_idx].y + 0.5 * ret[max_idx].h);
                center.w = (int)(Marginn.w + 0.5 * ret[max_idx].w);
                center.h = (int)(Marginn.h + 0.5 * ret[max_idx].h);
                return (int)(ret[max_idx].obj_id);
            }     
        }

        /* ---测试检测所花的时间--- */
        private void test_detector(string img_path)
        {
            //export class
            CvPoint center = new CvPoint();
            //CvPoint center_orig = new CvPoint();
            int ret = 0;
            int start = System.Environment.TickCount;
            ret = coal_gan_detect(img_path, out center); //, out center_orig);
            int end = System.Environment.TickCount;
            Console.WriteLine("return: {0}, pos: ({1}, {2}), cost time: {3} ms", ret, center.x, center.y, end - start);

            //弃用的c style 方法
            //ret = CPPDLL.Coal_Detection(img_path, 10, 200, ref center);
            //Console.WriteLine("ret: {0}, pos: ({1}, {2})", ret, center.x, center.y);
        }
        #endregion

        #region Camera
        /* ---软触发拍照--- */
        private void software_trigger(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_dev_A == null)
            {
                throw new InvalidOperationException("Camera Device is invalid");
            }
            try
            {
                trigger_start = System.Environment.TickCount;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                               System.Globalization.CultureInfo.InvariantCulture);
                logfile.WriteLine("拍照: " + timestamp);
                //执行软触发拍照
                m_dev_A.ExecuteSoftwareTrigger();
            }
            catch (Exception exception)
            {
                Catcher.Show(exception);
            }
        }

        /* ---相机打开回调--- */
        private void OnCameraOpen(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                btnOpen.Enabled = false;
                btnClose.Enabled = true;
            }));
        }

        /* ---相机关闭回调--- */
        private void OnCameraClose(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
            }));
        }

        /* ---相机丢失回调--- */
        private void OnConnectLoss_A(object sender, EventArgs e)
        {
            m_dev_A.ShutdownGrab();
            m_dev_A.Dispose();
            m_dev_A = null;

            logfile.WriteLine("----- 相机掉线 -----");

            this.Invoke(new Action(() =>
            {
                btnOpen.Enabled = true;
                btnClose.Enabled = false;
            }));
        }

        /* ---点击打开相机按钮--- */
        private void btnOpen_Click(object sender, EventArgs e)
        {
            //搜索可用的相机
            List<IDeviceInfo> li = Enumerator.EnumerateDevices();
            if (li.Count == 0)
            {
                MessageBox.Show(@"未搜索到任何可用摄像头设备，请检查您的设备连接");
                return;
            }
            open();

            if (is_software_trigger)
            {
                picture_trigger.Start();
            }

        }

        /* ---点击关闭相机按钮--- */
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_dev_A == null)
                {
                    throw new InvalidOperationException("Device A is invalid");
                }

                m_dev_A.StreamGrabber.ImageGrabbed -= OnImageGrabbed_A;
                m_dev_A.ShutdownGrab();
                m_dev_A.Close();
                m_dev_A = null;

                if (is_software_trigger)
                {
                    picture_trigger.Stop();
                    //onetime_trigger.Stop();
                }
            }
            catch (Exception exception)
            {
                Catcher.Show(exception);
            }
        }

        /* ---打开OPT摄像头--- */
        private void open()
        {
            try
            {
                if (m_dev_A == null)
                {
                    m_dev_A = Enumerator.GetDeviceByGigeIP(ipA);
                    // 注册链接时间
                    m_dev_A.CameraOpened += OnCameraOpen;
                    m_dev_A.ConnectionLost += OnConnectLoss_A;
                    m_dev_A.CameraClosed += OnCameraClose;
                    // 打开设备
                    if (!m_dev_A.Open())
                    {
                        MessageBox.Show(@"A点连接相机失败");
                    }
                    if(is_software_trigger)
                    {
                        m_dev_A.TriggerSet.Open(TriggerSourceEnum.Software);
                    }                    
                    else
                    {
                        m_dev_A.TriggerSet.Open(TriggerSourceEnum.Line1);
                    }
                    // 注册码流回调事件
                    m_dev_A.StreamGrabber.ImageGrabbed += OnImageGrabbed_A;
                    // 开启码流
                    if (!m_dev_A.GrabUsingGrabLoopThread())
                    {
                        MessageBox.Show(@"开启码流A失败");
                    }                    
                }
            }
            catch
            {
                MessageBox.Show(@"设备A开启失败，请检查ip");
            }
        }

        // 图像绘制
        private Graphics _gA = null;

        /* ---相机图片读取与处理--- */
        private void OnImageGrabbed_A(Object sender, GrabbedEventArgs e)
        {
            int process_start = System.Environment.TickCount;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                           System.Globalization.CultureInfo.InvariantCulture);

            logfile.WriteLine("触发: " + timestamp);

            // 转换帧数据为Bitmap
            var bitmap = e.GrabResult.ToBitmap(false);

            string time = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random ra = new Random();
            int ran = ra.Next(1, 999);
            string sourcefilename = sourcepath_A + time + "_" + ran + ".jpg";
            bitmap.Save(sourcefilename, System.Drawing.Imaging.ImageFormat.Jpeg);

            //Console.WriteLine("*** file saved: {0}", sourcefilename);
            int dual_time = 1000; // 统一到dual_time毫秒后发送坐标数据

            int im_w = bitmap.Width;
            int im_h = bitmap.Height;

            //************************* 检测图像是否静止，如果静止表示皮带停机，暂停检测，直接返回 **************
            // 设定图像中的局部区域进行统计，注意：区域宽度必须是4的倍数！
            /*
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(600, 200, 600, 800), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int iSumReg = 0;

            unsafe
            {
                byte* ptr = (byte*)(bitmapdata.Scan0);
                
                for (int i = 0; i < bitmapdata.Height * bitmapdata.Width; i++)
                {
                    iSumReg += ptr[i];
                    ptr += 1;
                }
                Console.WriteLine("+++++ SumReg: {0},{1}", iSumReg, iSumReg-g_iRegSum);

            }

            bitmap.UnlockBits(bitmapdata);

            if (System.Math.Abs(iSumReg-g_iRegSum)<160000) //停机阈值设置
            {
                File.Delete(sourcefilename);
                Console.WriteLine("++++++++++++++++++++++++ No Movement ++++++++++++++++++++");
                System.Timers.Timer onetime_trigger1;
                onetime_trigger1 = new System.Timers.Timer(1250); //设置时间间隔
                onetime_trigger1.Elapsed += new System.Timers.ElapsedEventHandler(software_trigger);
                onetime_trigger1.AutoReset = false;
                onetime_trigger1.Start();
                g_iRegSum = iSumReg;
                textBox_time.Text = "";// 皮带已停机";
                return;
            }            
            g_iRegSum = iSumReg;
            */

            // 显示读取的图片数据
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        if (_gA == null)
                        {
                            _gA = pbImage_A.CreateGraphics();
                            im_win_w_ = pbImage_A.Width;
                            im_win_h_ = pbImage_A.Height;
                            im_w_ = bitmap.Width;
                            im_h_ = bitmap.Height;
                            scale_x = (float)im_win_w_ / im_w_;
                            scale_y = (float)im_win_h_ / im_h_;
                        }

                        _gA.DrawImage(bitmap, new Rectangle(0, 0, pbImage_A.Width, pbImage_A.Height),
                            new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
                        bitmap.Dispose();

                        Pen pen1 = new Pen(Color.Green);
                        _gA.DrawLine(pen1, new Point(0, 85), new Point(860, 85));
                        _gA.DrawLine(pen1, new Point(0, 550), new Point(860, 550));

                    }
                    catch (Exception exception)
                    {
                        Catcher.Show(exception);
                    }
                }));
            }

            /************************* for test ***********************/
            //CvPoint  test_p = new CvPoint();
            //test_p.x = 1000;
            //test_p.y = 500; //图像坐标

            ////在图像上绘制点
            //this.Invoke(new Action(() =>
            //{
            //    int draw_x = (int)(test_p.x * ((float)(pbImage_A.Width) / (float)im_w));
            //    int draw_y = (int)(test_p.y * ((float)(pbImage_A.Height) / (float)im_h));

            //    SolidBrush redBrush = new SolidBrush(Color.Red);
            //    _gA.FillEllipse(redBrush, draw_x, draw_y, 8, 8);
            //}));

            //double test_tmpx = -0.3251 * test_p.x - 0.0027 * test_p.y + 1750 + 0.33 * 1.5 - 350;
            //coor_x = (int)test_tmpx;
            //double test_tmpy = 0.001 * test_p.x - 0.32764 * test_p.y + 440.2 + 50;
            //coor_y = (int)test_tmpy;

            //this.Invoke(new Action(() =>
            //{
            //    this.text_result.Text = "石头";
            //    this.textBox_index.Text = coor_x.ToString() + " " + coor_y.ToString();
            //    flag = 1;
            //    //WriteData_2();    
            //}));
            ////flag = 1;
            //threadTimer.Change(0, 1500); //立即触发送坐标

            //return;
            /************************ end ****************************/

            //煤矿分类结果
            CvPoint p = new CvPoint();
            //CvPoint p_orig = new CvPoint();
            int startTime = System.Environment.TickCount;

            //被弃用的C风格接口
            //int ret = CPPDLL.Coal_Detection(sourcefilename, 12, 233, ref p);

            Console.WriteLine("--- start detection ---");

            int ret = coal_gan_detect(sourcefilename, out p); //, out p_orig);

            Console.WriteLine("--- end detection ---");

            int TimeUsed = System.Environment.TickCount - startTime;

            this.Invoke(new Action(() =>
            {
                textBox_time.Text = TimeUsed.ToString() + "ms";
            }));

            logfile.WriteLine("Detected: x {0}; y {1}, @ Pic:{2} ", p.x, p.y, sourcefilename);

            string date = DateTime.Now.Date.ToShortDateString();

            // 换算机器人坐标位置
            double tmpx=0, tmpy=0;
            tmpx = -0.007 * p.x - 0.8188 * p.y + 992.3263 + 320+180;//-200;
            tmpy = -0.8175 * p.x + 0.0103 * p.y + 2128.4+40;

            if (ret == -1)
            {
                this.Invoke(new Action(() =>
                {
                    this.text_result.Text = "无";
                    this.textBox_index.Text = p.x.ToString() + " " + p.y.ToString();
                }));
                Console.WriteLine("=== None ===");

                _serial.GetSetting();
                if (_serial.IsOpen)
                {
                    WriteData_Light(false);
                }
            }

            else if (ret == -2) //检测到，但是距离不符合条件
            {
                this.Invoke(new Action(() =>
                {
                    this.text_result.Text = "有";
                    this.textBox_index.Text = p.x.ToString() + " " + p.y.ToString();
                }));
                Console.WriteLine("=== Detected!  ===");

                //save the results
                string save_path = Path.Combine(pred_results_root, date, "others");
                save_path = create_dir(save_path);
                FileInfo f = new FileInfo(sourcefilename);
                f.CopyTo(Path.Combine(save_path, f.Name));

                _serial.GetSetting();
                if (_serial.IsOpen)
                {
                    WriteData_Light(false);
                }
                /***********************查看系统时间，等待相差的时间的方式******************/
                int dual = System.Environment.TickCount - trigger_start;

                if (dual > dual_time)
                {
                    logfile.WriteLine("Error: 检测时间大于了 dual time");
                }

                else
                {
                    //Thread.Sleep(dual_time - dual);
                    do
                    {
                        Application.DoEvents();
                    } while (dual_time > (System.Environment.TickCount - trigger_start));
                }
            }

            else if (ret == -3) //检测到煤，但是距离不符合条件
            {
                this.Invoke(new Action(() =>
                {
                    this.text_result.Text = "煤";
                    this.textBox_index.Text = p.x.ToString() + " " + p.y.ToString();
                }));
                Console.WriteLine("=== Detected! But the distance is too close! ===");

                //save the results
                string save_path = Path.Combine(pred_results_root, date, "coal");
                save_path = create_dir(save_path);
                FileInfo f = new FileInfo(sourcefilename);
                f.CopyTo(Path.Combine(save_path, f.Name));

                _serial.GetSetting();
                if (_serial.IsOpen)
                {
                    WriteData_Light(true);
                }
                /***********************查看系统时间，等待相差的时间的方式******************/
                int dual = System.Environment.TickCount - trigger_start;

                if (dual > dual_time)
                {
                    logfile.WriteLine("Error: 检测时间大于了 dual time");
                }

                else
                {
                    //Thread.Sleep(dual_time - dual);
                    do
                    {
                        Application.DoEvents();
                    } while (dual_time > (System.Environment.TickCount - trigger_start));                
                }
            }

            else if (ret == 1)
            {
                Console.WriteLine("--- detection : 1 ---");
                /***********************查看系统时间，等待相差的时间的方式******************/
                int dual = System.Environment.TickCount - trigger_start;
                Console.WriteLine("dual time:{0}", dual);
                if (dual > dual_time)
                {
                    logfile.WriteLine("Error: 检测时间大于了 dual time");
                }
                
                else
                {
                    //Thread.Sleep(dual_time - dual);
                    do
                    {
                        Application.DoEvents();
                    } while (dual_time > (System.Environment.TickCount - trigger_start));
                    SendPoint((int)tmpx, (int)tmpy, counter);
                    counter = 1 - counter; //切换到另一个机器人；
                }

                Console.WriteLine("=== sent ===");

                this.Invoke(new Action(() =>
                {
                    this.text_result.Text = "煤";
                    this.textBox_index.Text = ((int)tmpx).ToString() + " " + ((int)tmpy).ToString();
                }));

                //save the results
                string save_path = Path.Combine(pred_results_root, date, "coal");
                save_path = create_dir(save_path);
                FileInfo f = new FileInfo(sourcefilename);
                f.CopyTo(Path.Combine(save_path, f.Name));

                //在图像上绘制点
                this.Invoke(new Action(() =>
                {
                    int draw_x = (int)(p.x * ((float)(pbImage_A.Width) / (float)im_w));
                    int draw_y = (int)(p.y * ((float)(pbImage_A.Height) / (float)im_h));

                    int rect_x = (int)((p.x - Marginn.w - p.w / 2) * ((float)(pbImage_A.Width) / (float)im_w));
                    rect_x = (rect_x > 0) ? rect_x : 0;
                    int rect_y = (int)((p.y - Marginn.h - p.h / 2) * ((float)(pbImage_A.Height) / (float)im_h));
                    rect_y = (rect_y > 0) ? rect_y : 0;
                    int rect_w = (int)((p.w + 2 * Marginn.w) * (float)(pbImage_A.Width) / (float)im_w);
                    rect_w = ((rect_x + rect_w) > pbImage_A.Width) ? (pbImage_A.Width - rect_x) : rect_w;
                    int rect_h = (int)((p.h + 2 * Marginn.h) * (float)(pbImage_A.Height) / (float)im_h);
                    rect_h = ((rect_y + rect_h) > pbImage_A.Height) ? (pbImage_A.Height - rect_y) : rect_h;

                    //Console.WriteLine("{0},{1},{2},{3}", rect_x, rect_y, rect_w, rect_h);
                    /*
                    int rect_x_d = (int)((p.x - p.w / 2) * ((float)(pbImage_A.Width) / (float)im_w));
                    rect_x_d = (rect_x_d > 0) ? rect_x_d : 0;
                    int rect_y_d = (int)((p.y - p.h / 2) * ((float)(pbImage_A.Height) / (float)im_h));
                    rect_y_d = (rect_y_d > 0) ? rect_y_d : 0;
                    int rect_w_d = (int)(p.w * (float)(pbImage_A.Width) / (float)im_w);
                    rect_w_d = ((rect_x_d + rect_w_d) > pbImage_A.Width) ? (pbImage_A.Width - rect_x_d) : rect_w_d;
                    int rect_h_d = (int)((p.h) * (float)(pbImage_A.Height) / (float)im_h);
                    */
                    //Console.WriteLine("{0},{1},{2},{3}", rect_x_d, rect_y_d, rect_w_d, rect_h_d);

                    SolidBrush redBrush = new SolidBrush(Color.Red);
                    _gA.FillEllipse(redBrush, draw_x, draw_y, 10, 10);

                    Pen greenpen = new Pen(Color.Green);
                    _gA.DrawRectangle(greenpen, rect_x, rect_y, rect_w, rect_h);

                    //Pen greenpen = new Pen(Color.Green);
                    //_gA.DrawRectangle(greenpen, rect_x_d, rect_y_d, rect_w_d, rect_h_d);
                }));

                _serial.GetSetting();
                if (_serial.IsOpen)
                {
                    WriteData_Light(true);
                }
            }
            else
            {
                Console.WriteLine("--- detection : 0 ---");
                /***********************查看系统时间，等待相差的时间的方式******************/
                int dual = System.Environment.TickCount - trigger_start;
                
                if (dual > dual_time)
                {
                    logfile.WriteLine("Error: 检测时间大于了 dual time");
                }
                
                else
                {
                    //Thread.Sleep(dual_time - dual);
                    do
                    {
                        Application.DoEvents();
                    } while (dual_time > (System.Environment.TickCount - trigger_start));
                    
                    SendPoint((int)tmpx, (int)tmpy, counter);
                    counter = 1 - counter; //切换到另一个机器人；
                }             
                //SendPoint((int)tmpx, (int)tmpy);

                Console.WriteLine("=== sent ===");

                this.Invoke(new Action(() =>
                {
                    this.text_result.Text = "矸";
                    this.textBox_index.Text = ((int)tmpx).ToString() + " " + ((int)tmpy).ToString();
                }));

                //save the results
                string save_path = Path.Combine(pred_results_root, date, "gan");
                save_path = create_dir(save_path);
                FileInfo f = new FileInfo(sourcefilename);
                f.CopyTo(Path.Combine(save_path, f.Name));

                //在图像上绘制点
                this.Invoke(new Action(() =>
                {
                    int draw_x = (int)(p.x * ((float)(pbImage_A.Width) / (float)im_w));
                    int draw_y = (int)(p.y * ((float)(pbImage_A.Height) / (float)im_h));

                    int rect_x = (int)((p.x - Marginn.w - p.w / 2) * ((float)(pbImage_A.Width) / (float)im_w));
                    rect_x = (rect_x > 0) ? rect_x : 0;
                    int rect_y = (int)((p.y - Marginn.h - p.h / 2) * ((float)(pbImage_A.Height) / (float)im_h));
                    rect_y = (rect_y > 0) ? rect_y : 0;
                    int rect_w = (int)((p.w + 2 * Marginn.w) * (float)(pbImage_A.Width) / (float)im_w);
                    rect_w = ((rect_x + rect_w) > pbImage_A.Width) ? (pbImage_A.Width - rect_x) : rect_w;
                    int rect_h = (int)((p.h + 2 * Marginn.h) * (float)(pbImage_A.Height) / (float)im_h);

                    //Console.WriteLine("{0},{1},{2},{3}", rect_x, rect_y, rect_w, rect_h);
                    /*
                    int rect_x_d = (int)((p.x - p.w / 2) * ((float)(pbImage_A.Width) / (float)im_w));
                    rect_x_d = (rect_x_d > 0) ? rect_x_d : 0;
                    int rect_y_d = (int)((p.y - p.h / 2) * ((float)(pbImage_A.Height) / (float)im_h));
                    rect_y_d = (rect_y_d > 0) ? rect_y_d : 0;
                    int rect_w_d = (int)(p.w * (float)(pbImage_A.Width) / (float)im_w);
                    rect_w_d = ((rect_x_d + rect_w_d) > pbImage_A.Width) ? (pbImage_A.Width - rect_x_d) : rect_w_d;
                    int rect_h_d = (int)((p.h) * (float)(pbImage_A.Height) / (float)im_h);
                    */

                    SolidBrush redBrush = new SolidBrush(Color.Red);
                    _gA.FillEllipse(redBrush, draw_x, draw_y, 10, 10);

                    Pen bulepen = new Pen(Color.Blue);
                    _gA.DrawRectangle(bulepen, rect_x, rect_y, rect_w, rect_h);
                    //_gA.DrawRectangle(bulepen, rect_x_d, rect_y_d, rect_w_d, rect_h_d);
                }));
            }

            //Console.WriteLine("file: {0}", sourcefilename);
            File.Delete(sourcefilename);

            System.Timers.Timer onetime_trigger;
            onetime_trigger = new System.Timers.Timer(700); //设置时间间隔
            onetime_trigger.Elapsed += new System.Timers.ElapsedEventHandler(software_trigger);
            onetime_trigger.AutoReset = false;
            onetime_trigger.Start();
        }
        #endregion

        #region 串口
        /* ---串口Port展示框--- */
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                btnClose.Enabled = false;
            }));
            string[] _portNameArray = SerialPort.GetPortNames();
            //添加到集合
            foreach (string s in _portNameArray)
            {
                comboBox_port.Items.Add(s);
            }
            //默认选择
            if (comboBox_port.Items.Count > 0)
            {
                comboBox_port.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("无有效串口！", "提示", MessageBoxButtons.OK);
            }
            comboBox_port.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /* ---设置串口--- */
        private int setSerialPort()
        {
            _serial.PortName = comboBox_port.Text;
            _serial.BaudRate = 9600;
            _serial.DataBits = 8; // 7;
            _serial.Parity = Parity.None; // Parity.Even;
            _serial.StopBits = StopBits.One;

            try
            {
                _serial.SetSerialPort();
            }
            catch
            {
                return 1;
            }
            return 0;
        }
        
        /* ---串口发送数据-煤--- */
        /*
        private void WriteData_1()
        {
            _serial.SendStr = "02 37 30 34 30 34 03 30 32";
            if (_serial.SendData(true) == 1)
            {
                MessageBox.Show("发送数据格式不正确！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string text = "";
            text += "煤识别数据发送成功   ";
            text += DateTime.Now.ToString("HH:mm:ss");
            text += "\r\n";
            textBox_Port.AppendText(text);
            textBox_Port.ScrollToCaret();
        }

        /* ---串口发送数据-矸石--- */
        /*
        private void WriteData_2()
        {
            _serial.SendStr = "02 37 30 35 30 34 03 30 33";
            if (_serial.SendData(true) == 1)
            {
                MessageBox.Show("发送数据格式不正确！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string text = "";
            text += "矸石识别数据发送成功   ";
            text += DateTime.Now.ToString("HH:mm:ss");
            text += "\r\n";
            textBox_Port.AppendText(text);
            textBox_Port.ScrollToCaret();
        }
        
        /* ---串口发送数据-综合--- */
        private void WriteData_Light(bool bOpen)
        {
            if (bOpen)
                _serial.SendStr = "7E FF 06 3A 00 01 00 EF";
            else
                _serial.SendStr = "7E FF 06 3A 00 00 01 EF";
            if (_serial.SendData(true) == 1)
            {
                MessageBox.Show("发送数据格式不正确！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string text = "";
            text += "数据发送成功   ";
            text += DateTime.Now.ToString("HH:mm:ss");
            text += "\r\n";
            textBox_Port.AppendText(text);
            textBox_Port.ScrollToCaret();
        }
        #endregion

        #region 界面按钮
        /* ---点击打开串口按钮--- */
        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            _serial.GetSetting();
            if (_serial.IsOpen)
            {
                _serial.CloseSerialPort();
                btnOpenPort.Text = "打开串口";
                string text = "";
                text += "串口已关闭   ";
                text += "\r\n";
                textBox_Port.AppendText(text);
                textBox_Port.ScrollToCaret();
            }
            else
            {
                if (this.setSerialPort() == 1)
                {
                    MessageBox.Show("无效串口！", "提示", MessageBoxButtons.OK);
                    return;
                }
                if (1 == _serial.OpenSerialPort())
                {
                    MessageBox.Show("打开串口失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string text = "";
                    text += "串口已打开   ";
                    text += "\r\n";
                    textBox_Port.AppendText(text);
                    textBox_Port.ScrollToCaret();

                    btnOpenPort.Text = "关闭串口";
                }
            }
        }

        /* ---点击发送数据按钮--- */
        private void btnSend_Click(object sender, EventArgs e)
        {
            //flag = 1;
            WriteData_Light(true);
        }

        /* ---当窗口被关闭--- */
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
            this.Dispose();
            this.Close();
        }

        /* ---向机器人发送测试坐标--- */
        private void button1_Click(object sender, EventArgs e)
        {
            int iMax = 705;
            int iSleepTime = 1000;
            for (int i = 700; i < iMax; i=i+10)
            {
                SendPoint(i,1350,0);
                SendPoint(i,1350, 1);
                Thread.Sleep(iSleepTime);
            }
        }

        /* ---链接数据库，重置煤块数据--- */ 
        private void button2_Click_1(object sender, EventArgs e)
        {  
            string nowaday = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                conn.Open();
                string sql_update = "update coal_count set number ='" + 0 + "' where time = '" + nowaday + "'";
                MySqlCommand cmd = new MySqlCommand(sql_update, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            this.textBox_count.Text = "0";
        }

        /* ---发送机器人启动暂停指令--- */
        //机器人0停止命令
        private void button3_Click(object sender, EventArgs e) 
        {
            SendPoint(-1, -1, 0);
        }
        //机器人0启动命令
        private void button4_Click(object sender, EventArgs e) 
        {
            SendPoint(0, -1, 0);
        }
        //机器人0急停命令
        private void button5_Click(object sender, EventArgs e) 
        {
            SendPoint(-1, 0, 0);
        }
        //机器人1启动命令
        private void button6_Click(object sender, EventArgs e) 
        {
            SendPoint(0, -1, 1);
        }
        //机器人1停止命令
        private void button7_Click(object sender, EventArgs e) 
        {
            SendPoint(-1, -1, 1);
        }
        //机器人1急停命令
        private void button8_Click(object sender, EventArgs e) 
        {
            SendPoint(-1, 0, 1);
        }
        #endregion
    }
}
