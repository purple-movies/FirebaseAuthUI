using DraconianMarshmallows.UI;
using DraconianMarshmallows.UI.Localization;
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
        [SerializeField] private Text promptMessage; 
        [SerializeField] private TextInput emailInput;
        [SerializeField] private TextInput passwordInput;
        [SerializeField] private ButtonPlus confirmButton;
        [SerializeField] private ButtonPlus navigationButton;

        public Action<string, string> OnProceed { get; internal set; }
        public Action OnNavigation { get; internal set; }

        public string PromptMessage { set { promptMessage.text = value; } }
        public string ConfirmButtonLabel { set { confirmButton.LabelText = value; } }
        public string NavigationButtonLabel {  set { navigationButton.LabelText = value; } }

        private Localizer localizer; 
        private bool emailValid = false;

        public override void Initialize(IParentUIController parentUIController)
        {
            base.Initialize(parentUIController);
            initializeEventCallbacks();
            initializeLocalization(parentUIController);
        }

        private void initializeEventCallbacks()
        {
            emailInput.onValueChanged.AddListener(validateEmail);
            confirmButton.onClick.AddListener(onClickProceed);
            navigationButton.onClick.AddListener(onClickNavigation);
        }

        private void initializeLocalization(IParentUIController parentUIController)
        {
            localizer = parentUIController.GetLocalizer();
            emailInput.PlaceHolder = localizer.GetLocalized("enter_email_address");
            passwordInput.PlaceHolder = localizer.GetLocalized("enter_password");
        }

        private void validateEmail(string email)
        {
            if (emailValid = isValidEmail(email)) emailInput.HideError();
            else emailInput.ShowError(localizer.GetLocalized("invalid_email_format"));
        }

        private void onClickProceed()
        {
            bool validationPassed = true;
            passwordInput.HideError();
            validateEmail(emailInput.text);

            if (passwordInput.text == "")
            {
                passwordInput.ShowError(localizer.GetLocalized("missing_password"));
                validationPassed = false;
            }

            if (!emailValid) validationPassed = false;

            if (!validationPassed) return; 
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
