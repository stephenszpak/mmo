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

            WriteInt32(playerId, packet, ref offset);
            WriteInt16(1, packet, ref offset);
            WriteFloat64(delta.x, packet, ref offset);
            WriteFloat64(delta.y, packet, ref offset);
            WriteFloat64(delta.z, packet, ref offset);

            Debug.Log($"Sending movement packet: {BitConverter.ToString(packet)}");

            udpClient.Send(packet, packet.Length, serverIP, serverPort);
        }

        private static void WriteInt32(int value, byte[] buffer, ref int offset)
        {
            int net = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes(net);
            Buffer.BlockCopy(bytes, 0, buffer, offset, 4);
            offset += 4;
        }

        private static void WriteInt16(short value, byte[] buffer, ref int offset)
        {
            short net = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes(net);
            Buffer.BlockCopy(bytes, 0, buffer, offset, 2);
            offset += 2;
        }

        private static void WriteFloat64(double value, byte[] buffer, ref int offset)
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
