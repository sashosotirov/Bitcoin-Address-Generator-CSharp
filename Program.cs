using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Cryptography;
using System.Numerics;
 

namespace bitcoinAddressCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            string hexHash = "0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6";

            byte[] PubKey = HexToByte(hexHash);
            Console.WriteLine("Public Key: " + ByteToHex(PubKey));
            
            byte[] PubKeySha = Sha256(PubKey);
            Console.WriteLine("Sha Public Key:" + ByteToHex(PubKeySha));
                       
            byte[] PubKeyShaRIPE = RipeMD160(PubKeySha);
            Console.WriteLine("RipeMD160 Sha Public Key: " + ByteToHex(PubKeyShaRIPE));

            byte[] PreHashWNetwork = AppendBitcoinNetwork(PubKeyShaRIPE, 0);
            Console.WriteLine("Public PreHashNetwork: " + ByteToHex(PreHashWNetwork));

            byte[] PublicHash = Sha256(PreHashWNetwork);
            Console.WriteLine("Public Hash: " + ByteToHex(PublicHash));

            byte[] PublicHashHash = Sha256(PublicHash);
            Console.WriteLine("Public Hash Hash: " + ByteToHex(PublicHashHash));

            byte[] Address = ConcatAddress(PreHashWNetwork, PublicHashHash);
            Console.WriteLine("Address is: " + ByteToHex(Address));

            Console.WriteLine("Human Address:" + Base58Encode(Address));
        }

        public static byte[] HexToByte(string HexString)
        {
            if (HexString.Length % 2 != 0)
            {
                throw new Exception("Invalid HEX");
            }

            byte[] retArray = new byte[HexString.Length / 2];
            for (int i = 0; i < retArray.Length; i++)
            {
                retArray[i]= byte.Parse(HexString.Substring(i*2,2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return retArray;  
        }
        public static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashString = new SHA256Managed();
            return hashString.ComputeHash(array);
        }
        public static byte[] RipeMD160(byte[] array)
        {
            RIPEMD160Managed hashString = new RIPEMD160Managed();
            return hashString.ComputeHash(array);
        }
        public static byte[] AppendBitcoinNetwork(byte[] RipeHash, byte Network)
        {
            byte[] extended = new byte[RipeHash.Length +1];
            extended[0] = (byte)Network;
            Array.Copy(RipeHash, 0, extended, 1, RipeHash.Length);
            return extended;
        }

        public static byte[] ConcatAddress(byte[] RipeHash, byte[] Checksum)
        {
            byte[] ret = new byte[RipeHash.Length + 4];
            Array.Copy(RipeHash, ret, RipeHash.Length);
            Array.Copy(Checksum, 0, ret, RipeHash.Length, 4);
            return ret;
        }

        private static string ByteToHex(byte[] pubKeySha)
        {
            string hex = BitConverter.ToString(pubKeySha);
            return hex;
        }
        public static string Base58Encode(byte[] array)
        {
            const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            string retString = string.Empty;
            BigInteger encodeSize = ALPHABET.Length;
            BigInteger arrayToInt = 0;
            for (int i = 0; i < array.Length; i++)
            {
                arrayToInt = arrayToInt * 256 + array[i];
            }
            while (arrayToInt > 0)
            {
                int rem = (int)(arrayToInt % encodeSize);
                arrayToInt /= encodeSize;
                retString = ALPHABET[rem] + retString;
            }
            for (int i = 0; i < array.Length && array[i] == 0; i++)
            {
                retString = ALPHABET[0] + retString;
            }
            return retString;
        }
    }
}
