using LiteNetLib;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var listener = new EventBasedNetListener();
            var client = new NetManager(listener);

            client.Start();
            client.Connect("localhost" , 9050, "SomeConnectionKey");
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
                dataReader.Recycle();
            };

            while (!Console.KeyAvailable)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }

            client.Stop();

        }
    }
}