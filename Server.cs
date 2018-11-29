using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace serverChat
{
    public static class Server
    {
        public static List<Client> Clients = new List<Client>();
        public static void NewClient(Socket handle)
        {
            try
            {
                Client newClient = new Client(handle);
                Clients.Add(newClient);
                Console.WriteLine("New client connected: {0}", handle.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Error with addNewClient: {0}.",exp.Message); }
        }
        public static void EndClient(Client client)
        {
            try
            {
                client.End();
                Clients.Remove(client);
                Console.WriteLine("User {0} has been disconnected.", client.UserName);
            }
            catch (Exception exp) { Console.WriteLine("Error with endClient: {0}.",exp.Message); }
        }
        public static void UpdateAllChats()
        {
            try
            {
                int countUsers = Clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    Clients[i].UpdateChat();
                }
            }
            catch (Exception exp) { Console.WriteLine("Error with updateAlLChats: {0}.",exp.Message); }
        }
        
    }
}
