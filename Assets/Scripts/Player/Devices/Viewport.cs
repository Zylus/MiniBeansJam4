using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : PlayerDevice
{
    public bool ToggleActive()
    {
        IsActive = !IsActive;
        
        if (IsActive)
        {
            CurrentEmissions = MaxEmissions;
        }
        else
        {
            CurrentEmissions = 0f;
        }

        return IsActive;
    }
}
