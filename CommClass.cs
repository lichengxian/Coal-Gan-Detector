using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace CoalMine
{
    public class CommClass
    {
        //定义字段成员
        private SerialPort _serialPort;
        private string _portName;   //串口名称
        private int _baudRate;      //串口波特率
        private int _dataBits;      //数据位
        private Parity _parity;     //校验位
        private StopBits _stopBits; //停止位
        private bool _isOpen;       //是否打开

        public string SendStr="";   //待发送的字符串
        public Byte[] Buffer=new Byte[1024];    //接收到的字节
        public Char[] StrBuf=new Char[1024];    //接收到的字符

        //定义属性成员
        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                _portName = value;
            }
        }
        public int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                _baudRate = value;
            }
        }
        public int DataBits
        {
            get
            {
                return _dataBits;
            }
            set
            {
                _dataBits = value;
            }
        }
        public Parity Parity
        {
            get
            {
                return _parity;
            }
            set
            {
                _parity = value;
            }
        }
        public StopBits StopBits
        {
            get
            {
                return _stopBits;
            }
            set
            {
                _stopBits = value;
            }
        }
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
        }

        //定义方法
        public CommClass()  //构造函数
        {
            _serialPort = new SerialPort();
            _serialPort.ReadTimeout = 50;
        }
        public void SetSerialPort()
        {
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = _baudRate;
            _serialPort.DataBits = _dataBits;
            _serialPort.Parity = _parity;
            _serialPort.StopBits = _stopBits;
        }

        public int OpenSerialPort() //打开串口0成功1失败
        {
            try
            {
                _serialPort.Open();
            }
            catch
            {
                return 1;
            }
            return 0;
        }

        public int CloseSerialPort()    //关闭串口0成功1失败
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch
            {
                return 1;
            }

            return 0;
        }
        public int SendData(bool _hex)  //将用户输入的字符串数，是否以十六进制发送
        {
            try
            {
                if (_hex)//以16进制发送
                {
                    string[] Str = SendStr.Trim().Split(' ');
                    Byte[] buf = new Byte[1024];
                    int i = 0;
                    foreach (string s in Str)
                    {
                        buf[i++] = Convert.ToByte(s,16);
                    }
                    _serialPort.Write(buf, 0, i);
                }
                else//以ASCII码格式发送
                {
                    char[] chs = SendStr.ToCharArray();
                    _serialPort.Write(chs, 0, chs.Length);
                }
            }
            catch
            {
                return 1;
            }
            return 0;
        }
        public int RecvData(bool _hex)//接收数据
        {
            int _bytes = 0;
            try
            {
                if (_hex)
                {
                    _bytes = _serialPort.Read(Buffer, 0, _serialPort.BytesToRead);
                }
                else
                {
                    _bytes = _serialPort.Read(StrBuf, 0, _serialPort.BytesToRead);
                }
            }
            catch
            {
                return 0;
            }
            return _bytes;
        }
        public int GetSetting()//获得当前打开串口参数
        {
            _portName = _serialPort.PortName;
            _baudRate = _serialPort.BaudRate;
            _dataBits = _serialPort.DataBits;
            _parity = _serialPort.Parity;
            _stopBits = _serialPort.StopBits;
            _isOpen = _serialPort.IsOpen;
            return 0;
        }
        public void DiscardInBuffer()
        {
            if(_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
            }
        }
    }
}
