using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomClient
{
    class Client
    {
        Socket clientSocket;

        public Client()
        {
            // 初始化
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        
        // 连接服务器
        public void Connected(string Ip,int port)
        {
            clientSocket.Connect(Ip,port);
            Console.WriteLine("连接成功");
            
            // 异步调用接收信息的线程
            Thread RecvThread = new Thread(RecvMessage);
            RecvThread.IsBackground = true;
            RecvThread.Start();
        }

        // 发送信息
       public void Send(String str)
        {
            clientSocket.Send(Encoding.Default.GetBytes(str));
        }

        // 接收信息
        private void RecvMessage()
        {
            try
            {
                byte[] strByte = new byte[500 * 1024];
                int len = clientSocket.Receive(strByte);
                Console.WriteLine(Encoding.Default.GetString(strByte, 0, len));
            }
            catch (Exception e) //服务器关闭
            {
                Console.WriteLine("服务器已关闭");
                Thread.CurrentThread.Abort();//关闭时切断进程
            }
            // 递归调用，让接收信息的线程一直在后台运行
            RecvMessage();
        }       

        // 启动客户端
        public void Run()
        {
            // 如果是在同一台电脑上运行，则这里是127.0.0.1即可
            // 如果是在不同电脑上，则这里需要填入运行服务端的主机的ip
            // 两台主机应当运行在同一局域网内
            // 校园网可能是不行的，可以换成手机热点
            Connected("127.0.0.1", 7788);
            string str = Console.ReadLine();
            while (!str.Equals("q"))
            {
                Send(str);
                str = Console.ReadLine();
            }
            Console.ReadLine();
        } 
    }
}
