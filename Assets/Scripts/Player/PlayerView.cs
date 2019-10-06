﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private Player _model;
    [SerializeField] private GameObject _playerShip;

    public void Init(Player model)
    {
        _model = model;
    }

    public void FixedUpdate()
    {
        if (_model.TimeUntilClearIsForgotten > 0)
        {
            _model.TimeUntilClearIsForgotten -= Time.deltaTime;
        }

        if (_model.TimeUntilClearIsForgotten < 0)
        {
           _model.TimeUntilClearIsForgotten = 0;
        }

        if (_model.TimeUntilOffenseIsForgotten > 0)
        {
            _model.TimeUntilOffenseIsForgotten -= Time.deltaTime;
        }
        
        if (_model.TimeUntilOffenseIsForgotten < 0)
        {
           _model.TimeUntilOffenseIsForgotten = 0;
        }

        if (_model.TimeUntilPlayerIsScanned > 0)
        {
            _model.TimeUntilPlayerIsScanned -= Time.deltaTime;
        }
        
        if (_model.TimeUntilPlayerIsScanned < 0)
        {
           _model.TimeUntilPlayerIsScanned = 0;
        }

        #region Input handling
        
        Vector2 oldPosition = transform.position;

        float translation = Input.GetAxis("Vertical");
        if (translation != 0 && _model.Devices["Engine"].IsActive)
        {
            float actualTranslation = _model.GetMovementMomentum(translation);
            transform.Translate(0,actualTranslation,0);
        }
        else
        {
            float actualTranslation = _model.CurrentSpeed;
            transform.Translate(0,actualTranslation,0);
        }
        
        float rotation = Input.GetAxis("Horizontal");
        if (rotation != 0 && _model.Devices["Engine"].IsActive)
        {
            float actualRotation = _model.GetActualRotation(rotation);
            transform.Rotate(0,0,actualRotation);
        }

        if (!_model.Devices["Engine"].IsActive || (translation == 0 && rotation == 0))
        {
            _model.DecayEngineHeat();
        }

        Vector2 newPosition = transform.position;
        _model.MovementVector = (newPosition - oldPosition).normalized;
        
        #endregion // Input handling
    }
}
