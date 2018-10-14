using DraconianMarshmallows.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
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

        private bool emailValid = false;

        protected override void Start()
        {
            base.Start();
            emailInput.onValueChanged.AddListener(onEmailChanged); 
            confirmButton.onClick.AddListener(onClickProceed);
            navigationButton.onClick.AddListener(onClickNavigation);
        }

        private void onEmailChanged(string email)
        {
            if (emailValid = isValidEmail(email))
                emailInput.HideError();
            else
                emailInput.ShowError("Please provide a valid email address");
        }

        private void onClickProceed()
        {
            if (!emailValid)
            {
                Debug.Log("Invalid email address...");
                return; 
            }
            OnProceed(emailInput.text, passwordInput.text);
        }

        private void onClickNavigation()
        {
            OnNavigation();
        }

        private bool isValidEmail(string email)
        {
            try
            {
                string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                  + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                  + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

                return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
