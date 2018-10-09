using DraconianMarshmallows.UI;
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
        //[SerializeField] private InputField emailInput;
        //[SerializeField] private InputField passwordInput;
        //[SerializeField] private Button confirmButton;
        //[SerializeField] private Button cancelButton;

        [SerializeField] private GameObject entryPoint; 
        [SerializeField] private GameObject loginUI; 
        [SerializeField] private GameObject registrationUI;
        [SerializeField] private Button registrationLink;

        protected override void Start()
        {
            base.Start();
            registrationLink.onClick.AddListener(onClickRegistration); 
        }

        private void onClickRegistration()
        {
            entryPoint.SetActive(false);
            registrationUI.SetActive(true);
        }
    }
}
