﻿using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace DraconianMarshmallows.FirebaseAuthUI
{
    public class AuthController : MonoBehaviour
    {
        #region Event Callbacks
        public Action<FirebaseUser> OnAuthenticationSuccess { get; internal set; }
        public Action OnInitializingFirebase { get; internal set; }
        public Action OnFirebaseInitializationFailure { get; internal set; }
        public Action OnFirebaseReady { get; internal set; }

        public Action OnInvalidPasswordForUser { get; internal set; }
        public Action OnUnexpectedError { get; internal set; }
        public Action OnInvalidEmailFormat { get; internal set; }
        public Action OnMissingPassword { get; internal set; }
        public Action OnPasswordTooWeak { get; internal set; }
        public Action OnNetworkError { get; internal set; }
        public Action OnAccountDisabled { get; internal set; }
        public Action OnAccountNotFound { get; internal set; }
        #endregion

        public bool FirebaseReady { get; private set; }

        private FirebaseApp app;
        private FirebaseAuth auth;

        private FirebaseUser currentUser;
        private bool firebaseInitialized; 

        protected virtual void Start()
        {
            StartCoroutine(waitForFirebase());

            if (OnInitializingFirebase != null) OnInitializingFirebase(); 

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp, i.e.
                    app = Firebase.FirebaseApp.DefaultInstance;
                    // where app is a Firebase.FirebaseApp property of your application class.

                    // Set a flag here indicating that Firebase is ready to use by your application.
                    auth = FirebaseAuth.DefaultInstance;
                    FirebaseReady = true; 
                    // Let Coroutine check this for callback ...

                    //OnFirebaseReady(); 
                    //Debug.Log("Loaded firebase dependencies...");
                }
                else
                {
                    // Firebase Unity SDK is not safe to use here.
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                    if (OnFirebaseInitializationFailure != null)
                        OnFirebaseInitializationFailure(); 
                }
            });
        }

        internal void StartLogin(string username, string password)
        {
            auth.SignInWithEmailAndPasswordAsync(username, password)
                .ContinueWith(task => onAuthRequest(task));
        }

        internal void RegisterNewUser(string username, string password)
        {
            auth.CreateUserWithEmailAndPasswordAsync(username, password)
                .ContinueWith(task => onAuthRequest(task));
        }

        private IEnumerator waitForFirebase()
        {
            while ( ! FirebaseReady)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log("Waiting for firebase .....");
            }
            OnFirebaseReady(); 
        }

        private void onAuthRequest(System.Threading.Tasks.Task<FirebaseUser> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Creating new account was cancelled...");
                return;
            }

            if (task.IsFaulted)
            {
                foreach(var innerException in task.Exception.InnerExceptions)
                    handleAuthException(innerException as FirebaseException); 

                return;
            }
            currentUser = task.Result;
            OnAuthenticationSuccess(currentUser);
        }

        private void handleAuthException(FirebaseException firebaseException)
        {
            switch ((AuthError) firebaseException.ErrorCode)
            {
                case AuthError.WrongPassword:
                case AuthError.EmailAlreadyInUse:
                case AuthError.AccountExistsWithDifferentCredentials:
                    OnInvalidPasswordForUser();
                    break;
                case AuthError.InvalidEmail:
                case AuthError.MissingEmail:
                    OnInvalidEmailFormat();
                    break;
                case AuthError.MissingPassword: OnMissingPassword();
                    break;
                case AuthError.WeakPassword: OnPasswordTooWeak();
                    break;
                case AuthError.UserNotFound: OnAccountNotFound();
                    break;
                case AuthError.NetworkRequestFailed: OnNetworkError();
                    break;
                case AuthError.UserDisabled: OnAccountDisabled();
                    break; 
                default: OnUnexpectedError();
                    break;
            }
        }
    }
}
