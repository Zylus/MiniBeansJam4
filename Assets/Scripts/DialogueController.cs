
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    Halt,
    Scan,
    FreeToGo,
    Hostile,
    Escaped,
    Fine
}

public class DialogueController : MonoBehaviour
{
    #region Singleton

    private static DialogueController _instance;
    public static DialogueController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    #endregion // singleton

    public string GetMessage(MessageType type)
    {
        switch(type)
        {
            case MessageType.Halt:
                return "[Halt, turn off your engine and await scanning]";
            case MessageType.Hostile:
                return "[You are under arrest, do not attempt to flee]";
            case MessageType.FreeToGo:
                return "[Thank you for complying, have a good day]";
            case MessageType.Escaped:
                return "[COMPUTER]:\n[It looks like you've escaped them for now, Captain.]";
            default:
                return string.Empty;
        }
    }
}
