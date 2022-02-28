using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KevinSkyba.EGHG.Data;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace KevinSkyba.Pandas
{
    public class PandasConnector
    {
        protected Logger logger;

        public enum ColumnType
        {
            Object,
            Int64,
            Float64,
            Bool,
            DateTime64,
            Vector2
        }

        [Serializable]
        public class Settings
        {
            public string Address => address;
            [SerializeField]
            [Tooltip("The address to bind the server on")]
            private string address = "127.0.0.1";

            public uint Port => port;
            [SerializeField]
            [Tooltip("The port the streaming server will listen on")]
            private uint port = 9060;

            public Settings(string address, uint port)
            {
                this.address = address;
                this.port = port;
            }
        }
        private Settings settings;

        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private NetworkStream tcpStream;

        private bool listening;

        [Serializable]
        class JsonContainer
        {
            public Dictionary<string, ColumnType> ColumnTypes;
            public Dictionary<string, object> ColumnValues;

            public JsonContainer()
            {
                ColumnTypes = new Dictionary<string, ColumnType>();
                ColumnValues = new Dictionary<string, object>();
            }
        }
        private JsonContainer jsonContainer;

        [Serializable]
        class JsonTransportContainer
        {
            [SerializeField]
            public List<string> Columns;
            [SerializeField]
            public List<ColumnType> Types;
            [SerializeField]
            public List<string> Values;

            public JsonTransportContainer()
            {
                Columns = new List<string>();
                Types = new List<ColumnType>();
                Values = new List<string>();
            }
        }
        private JsonTransportContainer jsonTransportContainer;

        [Serializable]
        class JsonTransportConstantsContainer
        {
            [SerializeField]
            public List<string> Keys;
            [SerializeField]
            public List<string> Values;

            public JsonTransportConstantsContainer()
            {
                Keys = new List<string>();
                Values = new List<string>();
            }
        }
        private JsonTransportConstantsContainer jsonTransportConstantsContainer;

        public PandasConnector(Settings settings, Dictionary<string, object> constants, ILogHandler logHandler = null)
        {
            this.settings = settings;
            jsonContainer = new JsonContainer();
            jsonTransportContainer = new JsonTransportContainer();
            jsonTransportConstantsContainer = new JsonTransportConstantsContainer();

            foreach (var item in constants)
            {
                jsonTransportConstantsContainer.Keys.Add(item.Key);
                jsonTransportConstantsContainer.Values.Add(item.Value.ToString());
            }

            if (logHandler != null)
            {
                logger = new Logger(logHandler);
            }
            else
            {
                logger = new Logger(Debug.unityLogger.logHandler);
            }

            StartServer();
        }

        ~PandasConnector()
        {
            StopServer();
        }

        private void StartServer()
        {
            if (!listening)
            {
                try
                {
                    IPAddress ip = IPAddress.Parse(settings.Address);
                    tcpListener = new TcpListener(ip, (int)settings.Port);
                    tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    tcpListener.Start();

                    tcpListener.BeginAcceptTcpClient(ClientConnected, null);
                    logger.Log(typeof(PandasConnector).ToString(), "Server started");

                    listening = true;
                }
                catch (Exception ex)
                {

                    logger.LogError(typeof(PandasConnector).ToString(), $"Server could not be started: {ex}");
                }
            }
        }

        public void RegisterColumn(string name, ColumnType type)
        {
            jsonContainer.ColumnTypes.Add(name, type);
        }

        public void WriteColumnValue(string name, object value)
        {
            if (jsonContainer.ColumnTypes.Keys.Contains(name))
                jsonContainer.ColumnValues[name] = value;
        }

        public void Send()
        {
            if (tcpStream != null && tcpClient != null && tcpClient.Connected)
            {
                jsonTransportContainer.Columns = jsonContainer.ColumnTypes.Keys.ToList();
                jsonTransportContainer.Types = jsonContainer.ColumnTypes.Values.ToList();
                jsonTransportContainer.Values.Clear();
                foreach (var columnName in jsonTransportContainer.Columns)
                {
                    if (jsonContainer.ColumnValues.ContainsKey(columnName))
                        jsonTransportContainer.Values.Add(jsonContainer.ColumnValues[columnName].ToString());
                    else
                        jsonTransportContainer.Values.Add(null);
                }
                string json = JsonUtility.ToJson(jsonTransportContainer);
                byte[] bytes = Encoding.ASCII.GetBytes(json);

                tcpStream.Write(bytes, 0, bytes.Length);
                tcpStream.Write(new byte[] { (byte)'\r' }, 0, 1); // Carriage Return

                jsonContainer.ColumnValues.Clear();
            }
            else if (tcpStream != null)
            {
                StopServer();
                StartServer();
            }
        }

        public void StopServer()
        {
            if (listening)
            {
                listening = false;

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

                logger.Log(typeof(PandasConnector).ToString(), "Server stopped");
            }
        }

        private void ClientConnected(IAsyncResult res)
        {
            logger.Log(typeof(PandasConnector).ToString(), "Client connected");

            // We only allow one client
            tcpClient = tcpListener.EndAcceptTcpClient(res);
            tcpStream = tcpClient.GetStream();

            // Send constants 
            string json = JsonUtility.ToJson(jsonTransportConstantsContainer);
            byte[] bytes = Encoding.ASCII.GetBytes(json);

            tcpStream.Write(bytes, 0, bytes.Length);
            tcpStream.Write(new byte[] { (byte)'\r' }, 0, 1); // Carriage Return
        }

    }
}