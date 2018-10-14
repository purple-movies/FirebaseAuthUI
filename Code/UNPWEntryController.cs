using DraconianMarshmallows.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DraconianMarshmallows.FirebaseAuthUI
{
    public class UNPWEntryController : UIBehavior
    {
        [SerializeField] private TextInput emailInput;
        [SerializeField] private TextInput passwordInput;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button navigationButton;

        public Action<string, string> OnProceed { get; internal set; }
        public Action OnNavigation { get; internal set; }

        protected override void Start()
        {
            base.Start();
            confirmButton.onClick.AddListener(onClickProceed);
            navigationButton.onClick.AddListener(onClickNavigation);
        }

        private void onClickProceed()
        {
            OnProceed(emailInput.text, passwordInput.text);
        }

        private void onClickNavigation()
        {
            OnNavigation();
        }
    }
}
