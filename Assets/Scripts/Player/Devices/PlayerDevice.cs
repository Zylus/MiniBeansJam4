using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmissionType
{
    Heat,
    Electro
}

public class PlayerDevice
{
    public string Name { get; set; }
    public float CurrentEmissions { get; set; }
    public float MaxEmissions { get; set; }
    public bool IsActive { get; set; }
    public EmissionType Emission { get; set; }
}
