using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WalletModule
{
    [CustomEditor(typeof(PlayerWallet))]
    public class PlayerWalletEditor : Editor
    {
        PlayerWallet script;
        string walletValue = "";

        private void OnEnable()
        {
            script = (PlayerWallet)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(); EditorGUILayout.Space();

            if (script.Wallet != null)
            {
                walletValue = "";
                for (int i = 0; i < Enum.GetNames(typeof(CurrencyType)).Length; i++)
                {
                    walletValue += String.Format("{0}\t{1}\n", (CurrencyType)i, script.Wallet[(CurrencyType)i].ToString());
                }
                EditorGUILayout.HelpBox(walletValue, MessageType.Info, true);
            }
            else
            {
                EditorGUILayout.HelpBox("Wallet value will be available in a Play mode", MessageType.Info);
            }
        }
    }
}