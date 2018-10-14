﻿using DraconianMarshmallows.UI;
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
        [SerializeField] private GameObject entryPoint;
        [SerializeField] private GameObject registrationPanel;
        [SerializeField] private UNPWEntryController loginUI; 
        [SerializeField] private UNPWEntryController registrationUI;
        [SerializeField] private Button registrationLink;

        protected override void Start()
        {
            base.Start();
            authController.OnAuthenticationSuccess += onRegistrationSuccessful; 

            loginUI.OnProceed += onStartLogin;
            loginUI.OnNavigation += onClickRegistration;
            registrationUI.OnProceed += onStartRegistration; 
            registrationUI.OnNavigation += onCancelRegistration;
        }

        private void onStartLogin(string username, string password)
        {
            Debug.Log("TODO::: On log in !");
            authController.StartLogin(username, password); 
        }

        private void onStartRegistration(string username, string password)
        {
            Debug.Log("Start registration...");
            authController.RegisterNewUser(username, password);
        }

        private void onRegistrationSuccessful(FirebaseUser firebaseUser)
        {
            Debug.Log("Registration successful..."); 
        }

        #region Navigation Callbacks
        private void onCancelRegistration()
        {
            registrationPanel.SetActive(false);
            entryPoint.SetActive(true);
        }

        private void onClickRegistration()
        {
            Debug.Log("Cliced register !!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            entryPoint.SetActive(false);
            registrationPanel.SetActive(true);
        }
        #endregion
    }
}
