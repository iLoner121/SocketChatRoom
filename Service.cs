using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace ChatRoomService
{
    class Service
    {
        Socket socketSevice ;
        // 用户列表
        List<Socket> userList;

        // 构造函数
        public Service()
        {
            // 初始化socket
           socketSevice = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           // 初始化用户列表
           userList = new List<Socket>();
        }

        // 启动服务器（监听端口）
        public void  Run()
        {
            // 打印IP
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            Console.WriteLine(AddressIP);
           
            // 监听指定端口
            // 这里使用IPAddress.Any是比较正确的写法，如果直接写127.0.0.1可能会导致不同设备连接出现问题
            socketSevice.Bind(new IPEndPoint(IPAddress.Any,7788));
            
            socketSevice.Listen(10);
            Console.WriteLine("服务器已启动");

            // 异步调用接收用户的线程
            Thread accThread = new Thread(Accept);
            accThread.IsBackground = true;
            accThread.Start();

            Console.ReadLine();
        }

        // 接受用户线程
        private void Accept()
        {
            //接受连接
            Socket clientSocket = socketSevice.Accept();
            userList.Add(clientSocket);
            //打印对应的IP地址
            Console.WriteLine(IPToAddress(clientSocket)+"已接入聊天室");

            // 异步调用对应用户的接收信息线程
            Thread RecvThread = new Thread(ReceMessage);
            RecvThread.IsBackground = true;
            RecvThread.Start(clientSocket);

            // 递归调用，保证接受用户的线程一直在运行
            Accept();
        }

        //接收客户端信息
        private void ReceMessage(Object obj)
        {
            Socket client = obj as Socket;
            byte[] strByte = new byte[1024 * 1024];
            string str = "";
            try
            {
              int len = client.Receive(strByte);
              str = Encoding.Default.GetString(strByte, 0, len);
              Broadcast(str,client);//广播给用户
              Console.WriteLine(str);
             }
             catch (Exception e)
             {
                // 取消该用户的连接
                Console.WriteLine(IPToAddress(client)+"退出");
                userList.Remove(client);
                Thread.CurrentThread.Abort();
            }
            // 递归调用
           ReceMessage(client); 
        }

        // 广播信息
        private void Broadcast(string userStr,object obj)
        {
            // 发信息的用户
            Socket clientSend = obj as Socket;
            foreach (Socket client in userList)
            {
                if (client != clientSend)
                {
                    client.Send(Encoding.Default.GetBytes(IPToAddress(clientSend)+":"+userStr));
                }
            }
        }

        private string IPToAddress(Socket soket)
        {
            return (soket.RemoteEndPoint as IPEndPoint).Address.ToString();
        }

    }
}
