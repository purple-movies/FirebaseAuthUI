using DraconianMarshmallows.UI;
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
        [SerializeField] private Button cancelButton;
    }
}
