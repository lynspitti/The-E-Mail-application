using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace The_E_Mail_application
{
    public class Symmetric_Encryption
    {
        private static string EncryptionKey = "Password1234";
        private static byte[] saltBytes = Encoding.UTF8.GetBytes("SalttlaS");

        public static string EncryptString(string Plain_Text)
        {
            RijndaelManaged Crypto = null;
            MemoryStream  MemStream = null;

            //Crypto streams allow for encryption in memory.
            CryptoStream Crypto_Stream = null;

            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            //Just grabbing the bytes since most crypto functions need bytes.
            byte[] PlainBytes = Byte_Transform.GetBytes(Plain_Text);

            try
            {
                Crypto = new RijndaelManaged();
                Rfc2898DeriveBytes p = new Rfc2898DeriveBytes(EncryptionKey, saltBytes);
                // sizes are devided by 8 because [ 1 byte = 8 bits ]
                Crypto.Key = p.GetBytes(Crypto.KeySize / 8);
                Crypto.IV = p.GetBytes(Crypto.BlockSize / 8);

                MemStream = new MemoryStream();

                //Calling the method create encryptor method Needs both the Key and IV these have to be from the original Rijndael call
                //If these are changed nothing will work right.
                ICryptoTransform Encryptor = Crypto.CreateEncryptor(Crypto.Key, Crypto.IV);

                //The big parameter here is the cryptomode.write, you are writing the data to memory to perform the transformation
                Crypto_Stream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);

                //The method write takes three params the data to be written (in bytes) the offset value (int) and the length of the stream (int)
                Crypto_Stream.Write(PlainBytes, 0, PlainBytes.Length);

            }
            finally
            {
                //if the crypto blocks are not clear lets make sure the data is gone
                if (Crypto != null) Crypto.Clear();
                //Close because of my need to close things when done.
                Crypto_Stream.Close();
            }
            //Return the memory byte array
            return Convert.ToBase64String(MemStream.ToArray());
        }

        public static string DecryptString(string Cipher_Text)
        {
            RijndaelManaged Crypto = null;
            MemoryStream MemStream = null;
            string Plain_Text;

            try
            {
                Crypto = new RijndaelManaged();
                Rfc2898DeriveBytes p = new Rfc2898DeriveBytes(EncryptionKey, saltBytes);
                // sizes are devided by 8 because [ 1 byte = 8 bits ]
                Crypto.Key = p.GetBytes(Crypto.KeySize / 8);
                Crypto.IV = p.GetBytes(Crypto.BlockSize / 8);

                MemStream = new MemoryStream(Convert.FromBase64String(Cipher_Text));

                //Create Decryptor make sure if you are decrypting that this is here and you did not copy paste encryptor.
                ICryptoTransform Decryptor = Crypto.CreateDecryptor(Crypto.Key, Crypto.IV);

                //This is different from the encryption look at the mode make sure you are reading from the stream.
                CryptoStream Crypto_Stream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);

                //I used the stream reader here because the ReadToEnd method is easy and because it return a string, also easy.
                StreamReader Stream_Read = new StreamReader(Crypto_Stream);
                Plain_Text = Stream_Read.ReadToEnd();
            }
            finally
            {
                if (Crypto != null) Crypto.Clear();
                MemStream.Flush();
                MemStream.Close();
            }
            return Plain_Text;
        }
    }
}
