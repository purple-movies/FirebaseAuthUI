using DraconianMarshmallows.UI;
using DraconianMarshmallows.UI.Localization;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DraconianMarshmallows.FirebaseAuthUI
{
    /// <summary> Main UI controller for Firebase Authentication UI. </summary>
    public class AuthUIController : ParentUIController
    {
        #region Inspector Fields
        [SerializeField] private AuthController authController;

        [SerializeField] private GameObject entryPoint;
        [SerializeField] private GameObject registrationPanel;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private Text loadingText;
 
        [SerializeField] private UNPWEntryController loginUI; 
        [SerializeField] private UNPWEntryController registrationUI; 
        [SerializeField] private Modal modal;
        #endregion
        private UNPWEntryController currentUNPWEntry;

        protected override void Start()
        {
            base.Start();
            initializeLocalization(); 

            #region Auth Controller Callbacks
            authController.OnInitializingFirebase += onInitializingFirebase; 
            authController.OnFirebaseReady += onFirebaseReady;
            authController.OnAuthenticationSuccess += onRegistrationSuccessful;
            #endregion

            #region Error Handlers
            authController.OnAccountDisabled += onAccountDisabled;
            authController.OnAccountNotFound += onAccountNotFound; 
            authController.OnInvalidEmailFormat += onInvalidEmailFormat;
            authController.OnInvalidPasswordForUser += onInvalidEmailForUser;
            authController.OnMissingPassword += onMissingPassword;
            authController.OnNetworkError += onNetworkError;
            authController.OnPasswordTooWeak += onPasswordTooWeak; 
            authController.OnUnexpectedError += onUnexpectedError;
            #endregion

            #region UI Callbacks
            loginUI.OnProceed += onStartLogin;
            loginUI.OnNavigation += onClickRegistration;
            registrationUI.OnProceed += onStartRegistration; 
            registrationUI.OnNavigation += onCancelRegistration;
            #endregion

            if (authController.FirebaseReady) onFirebaseReady();
            currentUNPWEntry = loginUI; 
        }

        private void initializeLocalization()
        {
            // TODO:: plug in other localized strings - everything's in EN-US right now. 
            localizer = Localizer.GetInstance("Localization/firebase_auth_enus", "Localization/firebase_auth_enus");

            // Initialize generic: 
            modal.Initialize(this); 
            loginUI.Initialize(this); 
            registrationUI.Initialize(this);

            // Override any generic localization:
            loginUI.PromptMessage = localizer.GetLocalized("sign_in_with_email_and_password");
            loginUI.ConfirmButtonLabel = localizer.GetLocalized("log_in"); 
            loginUI.NavigationButtonLabel = localizer.GetLocalized("or_register"); 

            registrationUI.PromptMessage = localizer.GetLocalized("sign_up_with_email_and_password");
            registrationUI.ConfirmButtonLabel = localizer.GetLocalized("continue");
            registrationUI.NavigationButtonLabel = localizer.GetLocalized("cancel");
        }

        private void onInitializingFirebase()
        {
            Debug.Log("Initializing firebase!!!!!"); 
            loadingUI.SetActive(true);
        }

        private void onFirebaseReady()
        {
            Debug.Log("Firebase is ready...");
            loadingUI.SetActive(false);
        }
        
        #region Authentication Process
        private void onStartLogin(string username, string password)
        {
            showLoading("Signing in...");
            authController.StartLogin(username, password); 
        }

        private void onStartRegistration(string username, string password)
        {
            showLoading("Signing up for account..."); 
            authController.RegisterNewUser(username, password);
        }

        private void onRegistrationSuccessful(FirebaseUser firebaseUser)
        {
            Debug.Log("Authentication successful : " + firebaseUser.UserId);
            hideLoading();
        }
        #endregion

        #region Navigation
        private void onClickRegistration()
        {
            currentUNPWEntry = registrationUI; 
            entryPoint.SetActive(false);
            registrationPanel.SetActive(true);
        }

        private void onCancelRegistration()
        {
            currentUNPWEntry = loginUI; 
            registrationPanel.SetActive(false);
            entryPoint.SetActive(true);
        }
        #endregion

        #region Error Handlers
        private void onUnexpectedError()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("unexpected_error_occured")); 
        }

        private void onPasswordTooWeak()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("passwords_too_weak"));
        }

        private void onNetworkError()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("network_error"));
        }

        private void onMissingPassword()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("missing_password"));
        }

        private void onInvalidEmailForUser()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("invalid_email_for_user"));
        }

        private void onInvalidEmailFormat()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("invalid_email_format"));
        }

        private void onAccountNotFound()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("account_not_found"));
        }

        private void onAccountDisabled()
        {
            modal.ShowInfoMessage(localizer.GetLocalized("account_disabled"));
        }
        #endregion

        private void showLoading(string message)
        {
            loadingText.text = message;
            loadingUI.SetActive(true);
        }

        private void hideLoading()
        {
            loadingUI.SetActive(false);
        }
    }
}
