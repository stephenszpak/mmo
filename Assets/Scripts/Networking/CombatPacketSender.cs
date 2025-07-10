using System.Net.Sockets;
using UnityEngine;

namespace MMO.Networking
{
    public class CombatPacketSender : MonoBehaviour
    {
        public string serverHost = "localhost";
        public int serverPort = 7777;

        private UdpClient udpClient;

        private void Start()
        {
            udpClient = new UdpClient();
        }

        public void SendAbilityUse(int abilitySlot)
        {
            byte[] data = System.BitConverter.GetBytes(abilitySlot);
            udpClient.Send(data, data.Length, serverHost, serverPort);
        }
    }
}
