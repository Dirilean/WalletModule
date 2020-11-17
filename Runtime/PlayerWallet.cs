using System;
using System.Collections.Generic;
using UnityEngine;

namespace WalletModule
{
    public class PlayerWallet : MonoBehaviour
    {
        //Public fields ------------------------------------------------------------------------------------------
        public static PlayerWallet instance;
        public Action<CurrencyType, uint> changingCurrencyAmount;
        public Dictionary<CurrencyType, uint> Wallet { get; private set; }
        public SavingWalletData savingModule;


        //Settings -----------------------------------------------------------------------------------------------
        [Header("Debug settings")]
        public bool debug = true;


        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                CreateWallet();
            }
        }

        public void CreateWallet()
        {
            Wallet = new Dictionary<CurrencyType, uint>();
            for (int i = 0; i < Enum.GetNames(typeof(CurrencyType)).Length; i++)
            {
                Wallet.Add((CurrencyType)i, 0);
            }

            if (debug) print("Created: " + WalletCash());
            changingCurrencyAmount += ChangingCurrencyAmount;
        }

        private void Start()
        {
            if (savingModule == null) savingModule = new SavingWalletData();

            var newWallet = new Dictionary<CurrencyType, uint>();
            savingModule.LoadWay.Invoke(out newWallet);
            Wallet = newWallet;

            if (savingModule.changeWalletValue != null) savingModule.changeWalletValue.Invoke(Wallet);
        }

        #region WalletOperations ------------------------------------------------------------------------------------------

        /// <summary>
        /// adding money to the player's game wallet
        /// </summary>
        /// <param name="type">Currency type</param>
        /// <param name="amount">Count of added money</param>
        public void AddCurrencyAmount(CurrencyType type, uint amount)
        {
            Wallet[type] += amount;
            if (debug) print(amount + " " + type.ToString() + " added: " + WalletCash());
            if (savingModule.changeWalletValue != null) savingModule.changeWalletValue.Invoke(Wallet);
        }

        /// <summary>
        /// removing money to the player's game wallet
        /// </summary>
        /// <param name="type">currency type</param>
        /// <param name="price">count of money for buy</param>
        /// <param name="OnSuccess">action after a successful purchase</param>
        /// <param name="OnDecline">action after rejecting</param>
        public void Buy(CurrencyType type, uint price, Action OnSuccess, Action OnDecline)
        {
            if (Wallet[type] >= price)
            {
                Wallet[type] -= price;
                OnSuccess.Invoke();
                if (savingModule.changeWalletValue != null) savingModule.changeWalletValue.Invoke(Wallet);
            }
            else
            {
                OnDecline.Invoke();
            }
        }
        #endregion


        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        public void ChangingCurrencyAmount(CurrencyType type, uint amount) => savingModule.changeWalletValue.Invoke(Wallet);

        /// <summary>
        /// Current cash amount
        /// </summary>
        /// <returns></returns>
        public string WalletCash()
        {
            return WalletSerialization.DictionaryToString(Wallet);
        }
    }
}