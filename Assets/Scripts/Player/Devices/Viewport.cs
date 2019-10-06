using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : PlayerDevice
{
    public float EngineBuildHeatRate { get; set; } = 0.3f;
    public float ViewportDecayRate { get; set; } = 0.15f;
    public float Emissions { get; set; } = 0.1f;

    public bool ToggleActive()
    {
        IsActive = !IsActive;
        return IsActive;
    }

    public float IncreaseViewportEmission()
    {
        float heatBuildUp = EngineBuildHeatRate;
        CurrentEmissions = Mathf.Lerp(CurrentEmissions, CurrentEmissions + heatBuildUp, 0.01f);

        if (CurrentEmissions > Emissions)
        {
            CurrentEmissions = Emissions;
        }

        return CurrentEmissions;
    }

    public float DecayViewportEmission()
    {
        if (CurrentEmissions > 0)
        {
            CurrentEmissions -= (ViewportDecayRate * Time.deltaTime);
            if (IsActive)
            {
                if (CurrentEmissions < Emissions)
                {
                    CurrentEmissions = Emissions;
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
