using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitUI
{
    public float Momentum { get; set; }
    public float HeatEmissions { get; set; }
    public float ElectroEmissions { get; set; }

    public void Init()
    {
        Momentum = GameController.Instance.Player.Model.CurrentSpeed;
        HeatEmissions = GameController.Instance.Player.Model.CurrentHeat;
        ElectroEmissions = GameController.Instance.Player.Model.CurrentElectro;
    }

    public void OnSpeedChanged(object sender, SpeedChangedEventArgs e)
    {
        Momentum = e.PlayerSpeed / e.MaxSpeed;
    }

    public void OnEnginePowerChanged(object sender, EnginePowerChangedEventArgs e)
    {
        HeatEmissions = e.EngineLoad / e.MaxLoad;
    }

    public void OnElectroEmissionsChanged(object sender, ElectroChangedEventArgs e)
    {
        ElectroEmissions = e.TotalElectro / e.MaxElectro;
    }
}
