using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;

namespace TextUtils
{
    public sealed class ProtoLogger2
    {
        private readonly Func<object, byte[]> _getBytes;
        private readonly IPEndPoint _ipEndPoint;

        // private readonly Layout _layout;
        private readonly UdpClient _udpClient;

        private static ProtoLogger2? _instance;


        // public Layout XmlEventLayout { get; }

        [NotNull]
        public static ProtoLogger2 Instance
        {
            get { return _instance ??= new ProtoLogger2(); }
        }

        private static UdpClient CreateUdpClient()
        {
            return new UdpClient
            {
                EnableBroadcast = true,
                Client =
                    new Socket(SocketType.Dgram, ProtocolType.Udp)
                    {
                        EnableBroadcast = true,
                        DualMode = true
                    }
            };
        }

        private ProtoLogger2()
        {
            // Log4JXmlEventLayoutRenderer xmlEventLayoutRenderer = new MyLog4JXmlEventLayoutRenderer();
            // XmlEventLayout = new MyLayout(xmlEventLayoutRenderer);
            _udpClient = CreateUdpClient();
            _ipEndPoint = new IPEndPoint(_protoLogIpAddress, _protoLogPort);

            // _layout = XmlEventLayout;
            // if (_layout == null)
            // {
            // throw new AppInvalidOperationException("Layout is null");
            // }

            _getBytes = DefaultGetBytes;
        }


        [NotNull]
        private byte[] DefaultGetBytes(object arg)
        {
            var encoding = Encoding.UTF8;
            return encoding.GetBytes(arg.ToString() ?? string.Empty);
        }

        public void LogAction(object info)
        {
            var bytes = _getBytes(info);
            var nBytes = bytes.Length;
            _udpClient.Send(bytes, nBytes, _ipEndPoint);
        }

        // public static readonly Action<LogEventInfo> ProtoLogAction = Instance.LogAction;
        

        private readonly int _protoLogPort=17771;
        private readonly IPAddress _protoLogIpAddress = IPAddress.Parse("10.25.0.103");

        // public static LogDelegates.LogMethod ProtoLogDelegate { get; } = ProtoLogMessage;

        // private static void ProtoLogMessage(string message)
        // {
        // ProtoLogAction(
        // LogEventInfo.Create(
        // LogLevel.Warn
        // , typeof(AppLoggingConfigHelper).FullName
        // , message
        // )
        // );
        // }
    }
}