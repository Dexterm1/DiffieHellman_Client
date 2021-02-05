using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiffieHellman_Client
{
    class Program
    {
        public static string privKey = "jfkgotmyvhs";
        public static string pubClientKey = "pcandxlr";
        public static byte[] pubServerKey = new byte[255];
        public static string combinedKey = privKey + pubClientKey;
        public static string newComboKey = "";

        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            int port = 13356;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            client.Connect(endPoint);

            byte[] keyBuffer = Encoding.UTF8.GetBytes(combinedKey);

            NetworkStream stream = client.GetStream();
            ReceiveMessage(stream);

            stream.Write(keyBuffer, 0, keyBuffer.Length);

            Console.WriteLine("Write message: ");
            string plainText = Console.ReadLine();
            string cipherText = EncryptByte(plainText, newComboKey);
            byte[] buffer = Encoding.UTF8.GetBytes(cipherText);

            stream.Write(buffer, 0, buffer.Length);

            Console.ReadKey();
        }

        public static async void ReceiveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            bool isRunning = true;

            while (isRunning)
            {
                int numberOfBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead);

                if (!receivedMessage.StartsWith("pcandxlr"))
                {
                    Console.WriteLine("Encrypted: " + receivedMessage);
                    string decryptText = DecryptByte(receivedMessage, newComboKey);
                    Console.WriteLine("Decrypted: " + decryptText);
                }
                else
                {
                    newComboKey = privKey + receivedMessage;
                    Console.WriteLine("SharedKey: " + newComboKey);
                }
            }
        }

        static string EncryptByte(string plainText, string key)
        {
            char[] chars = new char[plainText.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                if (plainText[i] == ' ')
                {
                    chars[i] = ' ';
                }
                else
                {
                    int j = plainText[i] - 97;
                    chars[i] = key[j];
                }
            }

            return new string(chars);
        }

        static string DecryptByte(string cipherText, string key)
        {
            char[] chars = new char[cipherText.Length];

            for (int i = 0; i < cipherText.Length; i++)
            {
                if (cipherText[i] == ' ')
                {
                    chars[i] = ' ';
                }
                else
                {
                    int j = key.IndexOf(cipherText[i]) + 97;
                    chars[i] = (char)j;
                }
            }

            return new string(chars);
        }
    }
}
