using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChangedEventArgs : EventArgs
{
    public float PlayerSpeed { get; set; }
    public float MaxSpeed { get; set; }
}

public class EnginePowerChangedEventArgs : EventArgs
{
    public float EngineLoad { get; set; }
    public float MaxLoad { get; set; }
}

public class ElectroChangedEventArgs : EventArgs
{
    public float TotalElectro { get; set; }
    public float MaxElectro { get; set; }
}

public class Player
{
    public const int DEFAULT_HEALTH = 100;
    public const float DEFAULT_MAX_SPEED = 0.1f;
    public const float DEFAULT_ACCELERATION = 0.06f;
    public const float DEFAULT_DECELERATION = 0.15f;
    public const float DEFAULT_ROTATION_SPEED = 80f;
    public const float DEFAULT_ELECTRO_DECAY_RATE = 0.4f;
    public const float DEFAULT_VIEWPORT_MAX_EMISSIONS = 0.2f;
    public const float DEFAULT_ENGINE_MAX_EMISSIONS = 1f;
    public const float DEFAULT_ENGINE_MIN_EMISSIONS = 0.1f;
    public const float DEFAULT_ENGINE_DECAY_RATE = 0.15f;
    public const float DEFAULT_ENGINE_BUILDUP_RATE = 1.5f;

    public int Health { get; set; }
    public float MaxSpeed { get; set; }
    public float CurrentSpeed { get; set; }
    public float Acceleration { get; set; }
    public float Deceleration { get; set; } // This is not a natural decay, but active "braking"
    public float RotationSpeed { get; set; }
    public float ElectroDecayRate { get; set; }
    private float _currentHeat;
    public float CurrentHeat { get { return GetCurrentHeat(); } }
    private float _currentElectro;
    public float CurrentElectro { get { return GetCurrentElectro(); } }
    public Dictionary<string, PlayerDevice> Devices { get; set; }
    public Dictionary<string, PlayerDevice> HeatDevices { get; set; }
    public Dictionary<string, PlayerDevice> ElectroDevices { get; set; }
    public Vector2 MovementVector { get; set; }
    public float TimeUntilClearIsForgotten { get; set; }
    public float TimeUntilOffenseIsForgotten { get; set; }
    public float TimeUntilPlayerIsScanned { get; set; }
    public bool PlayerIsBeingChecked { get; set; }
    public Mission ActiveMission { get; set; } 
    public int Cash { get; set; }

    public event EventHandler<SpeedChangedEventArgs> OnSpeedChanged;
    public event EventHandler<EnginePowerChangedEventArgs> OnEnginePowerChanged;
    public event EventHandler<ElectroChangedEventArgs> OnElectroChanged;

    public void Init()
    {
        Health = DEFAULT_HEALTH;
        CurrentSpeed = 0;
        MaxSpeed = DEFAULT_MAX_SPEED;
        Acceleration = DEFAULT_ACCELERATION;
        Deceleration = DEFAULT_DECELERATION;
        RotationSpeed = DEFAULT_ROTATION_SPEED;
        ElectroDecayRate = DEFAULT_ELECTRO_DECAY_RATE;
        InitializeDevices();
        ActiveMission = null;
        Cash = 0;
    }

    private void InitializeDevices()
    {
        Devices = new Dictionary<string, PlayerDevice>();
        HeatDevices = new Dictionary<string, PlayerDevice>();
        ElectroDevices = new Dictionary<string, PlayerDevice>();

        PlayerDevice engine = new Engine()
        {
            Name = "Engine",
            Emission = EmissionType.Heat,
            CurrentEmissions = DEFAULT_ENGINE_MIN_EMISSIONS,
            MaxEmissions = DEFAULT_ENGINE_MAX_EMISSIONS,
            IsActive = true,
            EngineBuildHeatRate = DEFAULT_ENGINE_BUILDUP_RATE,
            EngineDecayRate = DEFAULT_ENGINE_DECAY_RATE,
            MinEmissions = DEFAULT_ENGINE_MIN_EMISSIONS
        };

        Devices.Add("Engine", engine);
        HeatDevices.Add("Engine", engine);

        PlayerDevice viewport = new Viewport()
        {
            Name = "Viewport",
            Emission = EmissionType.Electro,
            CurrentEmissions = DEFAULT_VIEWPORT_MAX_EMISSIONS,
            MaxEmissions = DEFAULT_VIEWPORT_MAX_EMISSIONS,
            IsActive = true
        };

        Devices.Add("Viewport", viewport);
        ElectroDevices.Add("Viewport", viewport);


    }

    public float GetMovementMomentum(float translation)
    {
        if (translation > 0 && CurrentSpeed < MaxSpeed)
        {
            // accelerate
            CurrentSpeed += (Acceleration * Time.deltaTime);
            if (CurrentSpeed > MaxSpeed)
            {
                CurrentSpeed = MaxSpeed;
            }
            SpeedChanged();
        }
        else if (translation < 0 && CurrentSpeed > 0)
        {
            // brake
            CurrentSpeed -= (Deceleration * Time.deltaTime);
            if (CurrentSpeed < 0)
            {
                CurrentSpeed = 0;
            }
            SpeedChanged();
        }

        EnginePowerChanged(translation);
        return CurrentSpeed;
    }

    public void DecayEngineHeat()
    {
        (Devices["Engine"] as Engine).DecayEngineEmission();
        EnginePowerChanged(0f);
    }

    public float GetActualRotation(float rotation)
    {
        float actualRotation = rotation * RotationSpeed * -1;
        actualRotation *= Time.deltaTime;
        EnginePowerChanged(rotation, true);
        return actualRotation;
    }

    private float GetCurrentHeat()
    {
        float totalEmissions = 0f;
        foreach(KeyValuePair<string, PlayerDevice> kvp in HeatDevices)
        {
            totalEmissions += kvp.Value.CurrentEmissions;
        }
        
        return totalEmissions;
    }

    private float GetCurrentElectro()
    {
        float totalEmissions = 0f;
        foreach(KeyValuePair<string, PlayerDevice> kvp in ElectroDevices)
        {
            totalEmissions += kvp.Value.CurrentEmissions;
        }
        
        return totalEmissions;
    }

    private void SpeedChanged()
    {
        EventHandler<SpeedChangedEventArgs> handler = OnSpeedChanged;
        if (handler != null)
        {
            handler(this, new SpeedChangedEventArgs()
            {
                PlayerSpeed = this.CurrentSpeed,
                MaxSpeed = this.MaxSpeed
            });
        }
    }

    private void EnginePowerChanged(float amount, bool fromRotation = false)
    {
        (Devices["Engine"] as Engine).IncreaseEngineEmission(amount, fromRotation);

        EventHandler<EnginePowerChangedEventArgs> handler = OnEnginePowerChanged;
        if (handler != null)
        {
            handler(this, new EnginePowerChangedEventArgs()
            {
                EngineLoad = (Devices["Engine"] as Engine).CurrentEmissions,
                MaxLoad = 1
            });
        }
    }

    public void OnViewportToggled(object sender, EventArgs e)
    {
        (Devices["Viewport"] as Viewport).ToggleActive();

        EventHandler<ElectroChangedEventArgs> handler = OnElectroChanged;
        if (handler != null)
        {
            handler(this, new ElectroChangedEventArgs()
            {
                TotalElectro = CurrentElectro,
                MaxElectro = 1
            });
        }
    }

    public void OnEngineToggled(object sender, EventArgs e)
    {
        Devices["Engine"].IsActive = !Devices["Engine"].IsActive;
    }
}
