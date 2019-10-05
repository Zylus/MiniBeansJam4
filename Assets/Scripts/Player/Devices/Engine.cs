using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : PlayerDevice
{
    public float EngineBuildHeatRate { get; set; }
    public float EngineDecayRate { get; set; }
    public float MinEmissions { get; set; }

    public float IncreaseEngineEmission(float amount, bool fromRotation)
    {
        if (!fromRotation)
        {
            if (amount < 0)
            {
                amount = amount * -0.7f; //Braking doesn't create as much heat
            }
        }
        else
        {
            if (amount < 0)
            {
                amount = amount * -0.3f;
            }
            else
            {
                amount = amount * 0.3f;
            }
        }
        

        float heatBuildUp = amount * EngineBuildHeatRate;
        CurrentEmissions = Mathf.Lerp(CurrentEmissions, CurrentEmissions + heatBuildUp, 0.01f);

        if (CurrentEmissions > MaxEmissions)
        {
            CurrentEmissions = MaxEmissions;
        }

        return CurrentEmissions;
    }

    public float DecayEngineEmission()
    {
        if (CurrentEmissions > 0)
        {
            CurrentEmissions -= (EngineDecayRate * Time.deltaTime);
            if (IsActive)
            {
                if (CurrentEmissions < MinEmissions)
                {
                    CurrentEmissions = MinEmissions;
                }
            }
            else
            {
                if (CurrentEmissions < 0)
                {
                    CurrentEmissions = 0;
                }
            }
        }
        
        return CurrentEmissions;
    }
}
