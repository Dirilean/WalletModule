using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace WalletModule
{
    public class SavingWalletData
    {
        public Action<Dictionary<CurrencyType,uint>> changeWalletValue;

        private const string filePath = "D://testSaving.txt";
        private const string filePathBin = "D://testBinSaving.txt";

        public bool SavingInPlayerPrefs
        {
            get { return changeWalletValue.GetInvocationList().Any(x => x.Method.Name == "SaveInPlayerPrefs"); }
            set
            {
                if (value)
                    changeWalletValue += SaveInPlayerPrefs;
                else
                    changeWalletValue -= SaveInPlayerPrefs;
                Debug.Log("SavingInPlayerPrefs= "+value);
            }
        }
        public bool SavingInFile
        {
            get { return changeWalletValue.GetInvocationList().Any(x => x.Method.Name == "SaveInFile"); }
            set
            {
                if (value)
                    changeWalletValue += SaveInFile;
                else
                    changeWalletValue -= SaveInFile;
                Debug.Log("SavingInFile= " + value);
            }
        }
        public bool SavingInBinFile
        {
            get { return changeWalletValue.GetInvocationList().Any(x => x.Method.Name == "SaveBinaryInFile"); }
            set
            {
                if (value)
                    changeWalletValue += SaveBinaryInFile;
                else
                    changeWalletValue -= SaveBinaryInFile;
                Debug.Log("SavingInBinFile= " + value);
            }
        }
        public bool SavingOnServer
        {
            get { return changeWalletValue.GetInvocationList().Any(x => x.Method.Name == "SaveOnServer"); }
            set
            {
                if (value)
                    changeWalletValue += SaveOnServer;
                else
                    changeWalletValue -= SaveOnServer;
                Debug.Log("SaveOnServer= " + value);
            }
        }


        public delegate void load(out Dictionary<CurrencyType, uint> loadedWallet);
        public load LoadWay { get; private set; }
        public enum LoadingPreset
        {
            PlayerPrefs,
            File,
            BinFile,
            Server
        }

        public SavingWalletData(bool _savingInPlayerPrefs=true, bool _savingInFile=false, bool _savingInBinFile=false, bool _savingOnServer = false, LoadingPreset loadedPreset=LoadingPreset.PlayerPrefs)
        {
            SavingInPlayerPrefs = _savingInPlayerPrefs;
            SavingInFile = _savingInFile;
            SavingInBinFile = _savingInBinFile;
            SavingOnServer = _savingOnServer;

            switch(loadedPreset)
            {
                case LoadingPreset.PlayerPrefs: LoadWay = LoadPlayerPrefs; break;
                case LoadingPreset.File: LoadWay = LoadFromFile; break;
                case LoadingPreset.BinFile: LoadWay = LoadingFromBinary; break;
                case LoadingPreset.Server: LoadWay = LoadFromServer; break;
            }
        }

        #region Loading ------------------------------------------------------------------------------------

        /// <summary>
        /// Get PlayerPrefs saving
        /// </summary>
        static void LoadPlayerPrefs(out Dictionary<CurrencyType,uint> loadedWallet)
        {
            loadedWallet = new Dictionary<CurrencyType, uint>();
            for (int i = 0; i < Enum.GetNames(typeof(CurrencyType)).Length; i++)
            {
                loadedWallet[(CurrencyType)i] = Convert.ToUInt32(PlayerPrefs.GetString(((CurrencyType)i).ToString(), "0"));
            }
             Debug.Log("Loaded from PlayerPrefs");
        }

        /// <summary>
        /// get file saving
        /// </summary>
        /// <param name="loadedWallet"></param>
        static void LoadFromFile(out Dictionary<CurrencyType, uint> loadedWallet)
        {
            loadedWallet = new Dictionary<CurrencyType, uint>();
            using (FileStream fstream = File.OpenRead(filePath))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                loadedWallet = WalletSerialization.StringToDictionary(textFromFile);
                Debug.Log("Loaded from file "+filePath);
            }
        }

        static void LoadingFromBinary(out Dictionary<CurrencyType, uint> loadedWallet)
        {
            loadedWallet = new Dictionary<CurrencyType, uint>();
            using (FileStream fs = File.OpenRead(filePathBin))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Get count.
                int count = reader.ReadInt32();
                // Read in all pairs.
                for (int i = 0; i < count; i++)
                {
                    CurrencyType key = (CurrencyType)reader.ReadInt32();
                    uint value = reader.ReadUInt32();
                    loadedWallet.Add(key, value);
                }
            }
            Debug.Log("Loaded from binary file "+filePathBin);
        }

        //to do: create method for loading from server
        static void LoadFromServer(out Dictionary<CurrencyType, uint> loadedWallet)
        {
            Debug.LogError("Server loading not realisated yet");
            loadedWallet = null;
        }
        #endregion


        #region Saving ---------------------------------------------------------------------------------------
        public static void SaveInPlayerPrefs(Dictionary<CurrencyType, uint> currentWallet)
        {
            foreach (KeyValuePair<CurrencyType, uint> cur in currentWallet)
            {
                PlayerPrefs.SetString(cur.Key.ToString(), cur.Value.ToString());
            }
            Debug.Log("Save in PlayerPrefs");
        }
        public static void SaveInFile(Dictionary<CurrencyType, uint> currentWallet)
        {
            using (FileStream fstream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                //clear
                fstream.SetLength(0);
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(WalletSerialization.DictionaryToString(currentWallet));
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Debug.Log("Save in file "+filePath);
            }
        }
        public static void SaveBinaryInFile(Dictionary<CurrencyType, uint> currentWallet)
        {
            using (FileStream fs = File.OpenWrite(filePathBin))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                fs.SetLength(0);
                // Put count.
                writer.Write(currentWallet.Count);
                // Write pairs.
                foreach (var pair in currentWallet)
                {
                    writer.Write((int)pair.Key);
                    writer.Write(pair.Value);
                }
            }
            Debug.Log("Save binary in file " + filePathBin);
        }

        //to do: create another way to send on server without corutine
        public static void SaveOnServer(Dictionary<CurrencyType, uint> currentWallet)
        {
            Debug.LogError("Server saving not realisated yet");
            //StartCoroutine(Server.Post(Server.Api.walletSaving, JsonUtility.ToJson(PlayerWallet.instance.Wallet),
            //    delegate (UnityWebRequest www) { Debug.Log(www); },
            //    delegate { Debug.LogError("serverError"); }));
        }
        #endregion


        #region Cleaning------------------------------------------------------------------------------
        public static void ClearPlayerPrefs()
        {
            for (int i = 0; i < Enum.GetNames(typeof(CurrencyType)).Length; i++)
            {
                PlayerPrefs.DeleteKey(((CurrencyType)i).ToString());
            }
            Debug.Log("PlayerPrefs with currency was cleaned");
        }
        public static void ClearFile()
        {
            File.Delete(filePath);
            Debug.Log("File on " + filePath + " was deleted");
        }
        public static void ClearBinFile()
        {
            File.Delete(filePathBin);
            Debug.Log("File on " + filePathBin + " was deleted");
            
        }
        public static void ClearAllSavedData()
        {
            ClearPlayerPrefs();
            ClearFile();
            ClearBinFile();
        }
        #endregion


        #region OpenFiles -----------------------------------------------------------------------------
        public void OpenFile()
        {
            Application.OpenURL(filePath);
        }
        public void OpenBinFile()
        {
            Application.OpenURL(filePathBin);
        }
        #endregion
    }
}
