using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WalletModule
{
    public static class WalletSerialization
    {
        public static string DictionaryToString<K, V>(Dictionary<K, V> dictionary)
        {
            return (String.Join(", ", dictionary));
        }

        public static byte[] DictionaryToBin<K, V>(Dictionary<K, V> dictionary)
        {
            return System.Text.Encoding.Unicode.GetBytes(DictionaryToString(dictionary));
        }

        /*
        i cant convert string to K and V without other packages and utils -___-
        to do: make a universal serializer for dictionary

        public static Dictionary<K, V> StringToDictionary<K, V>(string str) where K : Enum where V : struct
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>();
            string[] arrayWithComma = str.Split(new char[] { '[', ']', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= arrayWithComma.Length / 2 + 1; i += 2)
            {
                //dictionary.OnDeserialization()
                Debug.Log(arrayWithComma[i]);
                Debug.Log(arrayWithComma[i + 1]);
                dictionary.Add((K)(object)arrayWithComma[i], (V)(object)arrayWithComma[i + 1]);
            }
            return null;
        }
        */

        public static Dictionary<CurrencyType, uint> StringToDictionary(string str)
        {
            Dictionary<CurrencyType, uint> dictionary = new Dictionary<CurrencyType, uint>();
            string[] arrayWithComma = str.Split(new char[] { '[', ']', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i <= arrayWithComma.Length / 2 + 1; i += 2)
            {
                //dictionary.OnDeserialization()
                //Debug.Log(Enum.Parse(typeof(CurrencyType),arrayWithComma[i]));
                //Debug.Log(uint.Parse(arrayWithComma[i + 1]));
                dictionary.Add(((CurrencyType)Enum.Parse(typeof(CurrencyType), arrayWithComma[i])) , uint.Parse(arrayWithComma[i + 1]));
            }
            return dictionary;
        }
    }
}
