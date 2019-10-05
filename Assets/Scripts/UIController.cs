using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    #region Singleton

    private static UIController _instance;
    public static UIController Instance { get { return _instance; } }

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

    public CockpitUIController CockpitUI { get; set; }

    public void InitializeCockpitUI()
    {
        CockpitUI = new CockpitUIController();
        CockpitUI.CreateCockpitUI();
    }
}
