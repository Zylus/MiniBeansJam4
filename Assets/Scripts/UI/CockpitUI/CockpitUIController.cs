using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitUIController
{
    public CockpitUIView View { get; set; }
    public CockpitUI Model { get; set; }

    public void CreateCockpitUI()
    {
        GameObject uiObjPrefab = Resources.Load<GameObject>("UI/CockpitUI");
        GameObject uiObj = GameObject.Instantiate(uiObjPrefab, new Vector2(0,0), Quaternion.identity);
        CockpitUI model = new CockpitUI();
        model.Init();
        GameController.Instance.Player.Model.OnSpeedChanged += model.OnSpeedChanged;
        GameController.Instance.Player.Model.OnEnginePowerChanged += model.OnEnginePowerChanged;
        GameController.Instance.Player.Model.OnElectroChanged += model.OnElectroEmissionsChanged;
        CockpitUIView view = uiObj.GetComponent<CockpitUIView>();
        view.Init(model);
        view.OnViewportToggledEvent += GameController.Instance.Player.Model.OnViewportToggled;
        view.OnEngineToggledEvent += GameController.Instance.Player.Model.OnEngineToggled;
        foreach(EnemyController enemy in GameController.Instance.Enemies)
        {
            enemy.View.MessageSendingEvent += view.OnMessageReceived;
        }
        this.View = view;
        this.Model = model;
    }
}
