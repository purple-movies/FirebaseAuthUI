using System;
using System.Collections;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace DraconianMarshmallows.FirebaseAuthUI
{
    public class AuthController : MonoBehaviour
    {
        private const string TAG = "AuthController";

        #region Event Callbacks
        public Action<FirebaseUser, bool> OnAuthenticationSuccess { get; internal set; }
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
        private bool newUserRegistration; 

        protected virtual void Start()
        {
            StartCoroutine(waitForFirebase());

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp, i.e.
                    app = FirebaseApp.DefaultInstance;
                    // where app is a Firebase.FirebaseApp property of your application class.

                    // Set a flag here indicating that Firebase is ready to use by your application.
                    //auth = FirebaseAuth.DefaultInstance;
                    //auth.StateChanged += onAuthenticationStateChanged;
                    FirebaseReady = true;
                    // Let Coroutine check this for callback ...

                    if (OnInitializingFirebase != null) OnInitializingFirebase();
                    Debug.Log("Loaded firebase dependencies...");

                    //onAuthenticationStateChanged(this, null);
                }
                else
                {
                    // Firebase Unity SDK is not safe to use here.
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                    if (OnFirebaseInitializationFailure != null)
                    {
                        OnFirebaseInitializationFailure();
                        return;
                    }
                }
            });
        }

        private void onAuthenticationStateChanged(object sender, EventArgs e)
        {
            if (auth.CurrentUser != currentUser)
            {
                bool signedIn = currentUser != auth.CurrentUser && auth.CurrentUser != null;
                if (!signedIn && currentUser != null)
                {
                    Debug.Log(TAG + ":: Signed out " + currentUser.UserId);
                }

                currentUser = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log(TAG + ":: Signed in " + currentUser.UserId);


                    Debug.Log(TAG + ":: The user's already authenticated ... :: \n" + currentUser.ToString());
                    OnAuthenticationSuccess(currentUser, newUserRegistration);


                    //displayName = currentUser.DisplayName ?? "";
                    //emailAddress = currentUser.Email ?? "";
                    //photoUrl = currentUser.PhotoUrl ?? "";
                }
            }
        }

        internal void StartLogin(string username, string password)
        {
            newUserRegistration = false;
            auth.SignInWithEmailAndPasswordAsync(username, password)
                .ContinueWith(task => onAuthRequest(task));
        }

        internal void RegisterNewUser(string username, string password)
        {
            newUserRegistration = true;
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
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += onAuthenticationStateChanged;
            onAuthenticationStateChanged(this, null);
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
            //currentUser = task.Result;
            Debug.Log("AuthController, Auth success:: \n" + currentUser.ToString());
            //OnAuthenticationSuccess(currentUser, newUserRegistration);
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
