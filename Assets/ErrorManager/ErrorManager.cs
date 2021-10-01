using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class ErrorManager : MonoBehaviour
{
    [SerializeField] private GameObject scriptErrorBackground;
    [SerializeField] private Text scriptExceptionText;

    private static readonly bool DisplayScriptErrors = true;

    private static readonly StringBuilder ErrorBuilder = new StringBuilder();

    private static event Action errorBuilderUpdated;

    private void Awake()
    {
        scriptErrorBackground.SetActive(false);

        // If we've already caught an error, display it immediately
        if (ErrorBuilder.Length > 0)
        {
            OnErrorBuilderUpdated();
        }

        // Listen for future errors
        errorBuilderUpdated += OnErrorBuilderUpdated;

        // Option to disable script exceptions in release builds
        // #if !DEVELOPMENT_BUILD
        //             // Disable script exception display outside of dev builds
        //             displayScriptExceptions = false;
        // #endif
    }

    private void OnErrorBuilderUpdated()
    {
        scriptErrorBackground.SetActive(true);
        scriptExceptionText.text = ErrorBuilder.ToString();
        scriptExceptionText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // Currently this event is unsubscribed when any ErrorManager instance is destroyed
        // This implementation would prevent multiple ErrorManagers from functioning properly
        Application.logMessageReceived -= OnLogMessageReceived_AddErrorToBuilder;

        errorBuilderUpdated -= OnErrorBuilderUpdated;
    }

    private static void OnLogMessageReceived_AddErrorToBuilder(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            // Prepend the message
            ErrorBuilder.Insert(0, condition + "\n\n" + stackTrace + "\n\n");
            errorBuilderUpdated?.Invoke();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadErrorManager()
    {
        if (DisplayScriptErrors)
        {
            // subscribe before scene load so that all errors will be caught
            Application.logMessageReceived += OnLogMessageReceived_AddErrorToBuilder;
        }
    }

    // Comment in to test panel display
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         string s = null;
    //         int i = s.Length;
    //     }
    // }
}

