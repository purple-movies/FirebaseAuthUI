﻿using DraconianMarshmallows.UI;
using DraconianMarshmallows.UI.Localization;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DraconianMarshmallows.FirebaseAuthUI
{
    /**
     * Main UI controller for Firebase Authentication UI.  
     */
    public class AuthUIController : UIBehavior
    {
        [SerializeField] private AuthController authController;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private Text loadingText;
        [SerializeField] private GameObject entryPoint; 
        [SerializeField] private GameObject registrationPanel; 
        [SerializeField] private UNPWEntryController loginUI; 
        [SerializeField] private UNPWEntryController registrationUI; 
        [SerializeField] private Button registrationLink;

        private UNPWEntryController currentUNPWEntry;
        private Localizer localizer; 

        protected override void Start()
        {
            base.Start();
            // TODO:: plug in other localized strings - everything's in EN-US.
            localizer = Localizer.GetInstance("Localization/firebase_auth_enus", "Localization/firebase_auth_enus");

            Debug.Log("TEST:: " + localizer.GetLocalized("signing_in"));

            authController.OnFirebaseReady += onFirebaseReady;
            authController.OnAuthenticationSuccess += onRegistrationSuccessful;

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

            loginUI.OnProceed += onStartLogin;
            loginUI.OnNavigation += onClickRegistration;
            registrationUI.OnProceed += onStartRegistration; 
            registrationUI.OnNavigation += onCancelRegistration;

            if (authController.FirebaseReady) onFirebaseReady();
            currentUNPWEntry = loginUI; 
        }

        private void onFirebaseReady()
        {
            Debug.Log("Firebase is ready...");
            loadingUI.SetActive(false);
        }

        private void showErrorDialog(string message)
        {
            Debug.LogWarningFormat("Show error message:: %s", message); 
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
            showErrorDialog(localizer.GetLocalized("unexpected_error_occured")); 
        }

        private void onPasswordTooWeak()
        {
            showErrorDialog(localizer.GetLocalized("passwords_too_weak"));
        }

        private void onNetworkError()
        {
            throw new NotImplementedException();
        }

        private void onMissingPassword()
        {
            throw new NotImplementedException();
        }

        private void onInvalidEmailForUser()
        {
            throw new NotImplementedException();
        }

        private void onInvalidEmailFormat()
        {
            throw new NotImplementedException();
        }

        private void onAccountNotFound()
        {
            throw new NotImplementedException();
        }

        private void onAccountDisabled()
        {
            throw new NotImplementedException();
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
