using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MMO.Networking
{
    [DisallowMultipleComponent]
    public class PlayerNetworkController : MonoBehaviour
    {
        public int playerId = 1;
        public string serverIP = "127.0.0.1";
        public int serverPort = 4000;

        private UdpClient udpClient;
        private Vector3 lastPosition;

        private void Start()
        {
            udpClient = new UdpClient();
            lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 currentPosition = transform.position;
            Vector3 delta = currentPosition - lastPosition;
            if (delta.sqrMagnitude > 0f)
            {
                SendMovement(delta);
                lastPosition = currentPosition;
            }
        }

        private void SendMovement(Vector3 delta)
        {
            byte[] packet = new byte[30];
            int offset = 0;

            int pidNet = IPAddress.HostToNetworkOrder(playerId);
            short opcodeNet = IPAddress.HostToNetworkOrder((short)1);

            byte[] tmp = BitConverter.GetBytes(pidNet);
            Buffer.BlockCopy(tmp, 0, packet, offset, 4);
            offset += 4;

            tmp = BitConverter.GetBytes(opcodeNet);
            Buffer.BlockCopy(tmp, 0, packet, offset, 2);
            offset += 2;

            WriteDouble(delta.x, packet, ref offset);
            WriteDouble(delta.y, packet, ref offset);
            WriteDouble(delta.z, packet, ref offset);

            udpClient.Send(packet, packet.Length, serverIP, serverPort);
        }

        private static void WriteDouble(double value, byte[] buffer, ref int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Buffer.BlockCopy(bytes, 0, buffer, offset, 8);
            offset += 8;
        }

        private void OnDestroy()
        {
            udpClient?.Close();
        }
    }
}
