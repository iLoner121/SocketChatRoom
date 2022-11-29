using ChatRoomClient;
using ChatRoomService;

class Program {
    public static void Main(String[] args) {
        Console.WriteLine("输入0以启动服务端，输入1以启动客户端");
        while(true) {
            string a=Console.ReadLine();
            int b=int.Parse(a);//创建int变量接收转换后的值
            if(b==0) {
                Service service = new Service();
                service.Run();
            } else if(b==1) {
                Client client = new Client();
                client.Run();
            } else {
                Console.WriteLine("输入错误，请重新输入");
            }
        }
        
    }
}