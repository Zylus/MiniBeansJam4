
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

    private List<string> MISSION_MESSAGES = new List<string>()
    {
        "[PLANET]\nI need you to bring these units of {0} to {1} right now.",
        "[PLANET]\nWould you kindly deliver these units of {0} to {1}?",
        "[PLANET]\nI heard you can be quite discreet. I need you to get these {0} to {1}."
    };

    public string GetMessage(MessageType type)
    {
        switch(type)
        {
            case MessageType.Halt:
                return "[AUTHORITY]\nUnknown vessel XR31, this is the Federal authority! Please stop immediately for a routine inspection.";
            case MessageType.Hostile:
                return "[AUTHORITY]\nPursuing vessel XR31! Pilot, do NOT attempt to resist arrest!";
            case MessageType.FreeToGo:
                return "[AUTHORITY]\nAll clear. You may proceed.";
            case MessageType.Escaped:
                return "[COMPUTER]\nYou are out of their signal range, Captain. Remain alert.";
            case MessageType.Fine:
                return "[AUTHORITY]\nGot you, scum! We're disposing of your illegal goods and adding a punitive fine to your account. Please stand by.";
            case MessageType.Scan:
                return "[AUTHORITY]\nWe’ll need to perform a routine scan of your cargo. Please do not resist.";
            default:
                return string.Empty;
        }
    }

    public string GetMissionMessage(Mission mission)
    {
        string baseMessage = MISSION_MESSAGES[Random.Range(0,MISSION_MESSAGES.Count)];
        return string.Format(baseMessage, mission.CargoName, mission.EndStation.name);
    }
}
