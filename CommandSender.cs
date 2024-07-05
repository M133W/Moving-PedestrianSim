using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class CommandSender : MonoBehaviour
{
    public string ipAddress = "127.0.0.1"; 
    public int port = 8888;

    public void SendCommand(string command)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            TcpClient client = new TcpClient(ipAddress, port);

            NetworkStream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            stream.Close();
            client.Close();

            Debug.Log("Command sent successfully: " + command);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending command: " + e.Message);
        }
    }
}
