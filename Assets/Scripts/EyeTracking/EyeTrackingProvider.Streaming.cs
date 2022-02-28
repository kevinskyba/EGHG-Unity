using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace EyeTracking
        {
            public abstract partial class EyeTrackingProvider : MonoBehaviour
            {

                /// <summary>
                /// Streaming allows the device to stream its <see cref="HeadGaze"/> and <see cref="EyeGaze"/> data
                /// to tcp clients.
                /// </summary>
                [Serializable]
                public class Streaming
                {
                    public bool AutoStartStreaming => autoStartStreaming;
                    [SerializeField]
                    [Tooltip("Whether this provider should automatically start streaming data")]
                    private bool autoStartStreaming = false;

                    public string StreamingAddress => streamingAddress;
                    [SerializeField]
                    [Tooltip("The address to bind the server on")]
                    private string streamingAddress = "127.0.0.1";

                    public uint StreamingPort => streamingPort;
                    [SerializeField]
                    [Tooltip("The port the streaming server will listen on")]
                    private uint streamingPort = 9060;
                }
                [SerializeField]
                [InspectorName("Recording")]
                private Streaming streamingSettings;

                [SerializeField]
                private bool isStreaming;

                public bool IsStreaming => isStreaming;

                private TcpListener tcpListener;
                private TcpClient tcpClient;
                private NetworkStream tcpStream;

                public void StartStreaming()
                {
                    if (!isStreaming)
                    {
                        try
                        {
                            IPAddress ip = IPAddress.Parse(streamingSettings.StreamingAddress);
                            tcpListener = new TcpListener(ip, (int)streamingSettings.StreamingPort);
                            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                            tcpListener.Start();

                            tcpListener.BeginAcceptTcpClient(ClientConnected, null);
                            logger.Log(typeof(EyeTrackingProvider).ToString(), "Streaming server started");

                            isStreaming = true;
                        }
                        catch (Exception ex)
                        {

                            logger.LogError(typeof(EyeTrackingProvider).ToString(), $"Streaming server could not be started: {ex}");
                        }
                    }
                }

                private void ClientConnected(IAsyncResult res)
                {
                    logger.Log(typeof(EyeTrackingProvider).ToString(), "Client connected");

                    // We only allow one client
                    tcpClient = tcpListener.EndAcceptTcpClient(res);
                    tcpStream = tcpClient.GetStream();
                }

                public void StreamData(RecordingData.RecordEntry entry)
                {
                    if (tcpStream != null && tcpClient != null && tcpClient.Connected)
                    {
                        string json = JsonUtility.ToJson(entry);
                        byte[] bytes = Encoding.ASCII.GetBytes(json);

                        tcpStream.Write(bytes, 0, bytes.Length);
                        tcpStream.Write(new byte[] { (byte)'\r' }, 0, 1); // Carriage Return
                    }
                    else if (tcpStream != null)
                    {
                        StopStreaming();
                        StartStreaming();
                    }
                }

                public void StopStreaming()
                {
                    if (isStreaming)
                    {
                        isStreaming = false;

                        if (tcpStream != null)
                        {
                            tcpStream.Close();
                            tcpStream = null;
                        }

                        if (tcpClient != null)
                        {
                            tcpClient.Close();
                            tcpClient = null;
                        }

                        if (tcpListener != null)
                        {
                            tcpListener.Stop();
                            tcpListener = null;
                        }

                        logger.Log(typeof(EyeTrackingProvider).ToString(), "Streaming server stopped");
                    }
                }
            }
        }
    }
}