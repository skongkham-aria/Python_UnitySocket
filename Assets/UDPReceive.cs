// This script is designed to be attached to a GameObject in a Unity scene.
// It creates a separate thread to listen for UDP packets on a specified port
// without blocking the main game loop.

using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    // The thread that will run the UDP listener
    private Thread receiveThread;
    
    // The UDP client that will listen for incoming data
    private UdpClient client;

    // --- Public variables that can be set in the Unity Inspector ---

    // The port to listen on. Default is 5052.
    public int port = 5052;
    
    // A flag to control the listening loop.
    public bool startReceiving = true;
    
    // If true, received messages will be printed to the Unity console.
    public bool printToConsole = false;
    
    // The most recently received data, accessible from other scripts.
    // 'volatile' ensures that the value is always read from main memory,
    // which is important when a variable is accessed by multiple threads.
    public volatile string data;

    // This method is called when the script instance is being loaded.
    public void Start()
    {
        // Create a new thread for the ReceiveData method
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        
        // Set IsBackground to true so the thread will be terminated when the application quits.
        receiveThread.IsBackground = true;
        
        // Start the thread
        receiveThread.Start();
    }

    // This private method runs on the separate thread and handles the UDP receiving logic.
    private void ReceiveData()
    {
        // Create a new UdpClient to listen on the specified port.
        client = new UdpClient(port);
        
        // The main listening loop. It continues as long as startReceiving is true.
        while (startReceiving)
        {
            try
            {
                // Set up an IPEndPoint to receive data from any IP address on any port.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                
                // Blocks until a UDP datagram is received. 
                // The received data is stored in the dataByte array.
                byte[] dataByte = client.Receive(ref anyIP);
                
                // Convert the received byte array into a UTF8 string.
                data = Encoding.UTF8.GetString(dataByte);

                // If printToConsole is enabled, log the received data to the Unity console.
                if (printToConsole)
                {
                    // Note: Debug.Log is thread-safe and can be called from other threads.
                    Debug.Log(data);
                }
            }
            catch (Exception err)
            {
                // If an error occurs (e.g., the socket is closed), print the error message.
                Debug.LogError(err.ToString());
            }
        }
    }
    
    // This method is called when the application quits.
    // It's important to properly clean up the thread and client.
    private void OnApplicationQuit()
    {
        // Stop the listening loop
        startReceiving = false;

        // Abort the thread if it's running
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        // Close the UDP client
        if (client != null)
        {
            client.Close();
        }
    }
}
