using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private Player _model;
    [SerializeField] private GameObject _playerShip;
    [SerializeField] private GameObject _forwardFlame;
    [SerializeField] private GameObject _backwardFlame;
    [SerializeField] private GameObject _rightTurnFlame;
    [SerializeField] private GameObject _leftTurnFlame;

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

        _model.UpdateViewportEmissions();

        #region Input handling
        
        Vector2 oldPosition = transform.position;

        float translation = Input.GetAxis("Vertical");
        if (translation != 0 && _model.Devices["Engine"].IsActive)
        {
            float actualTranslation = _model.GetMovementMomentum(translation);
            transform.Translate(0,actualTranslation,0);
            if (translation > 0)
            {
                _forwardFlame.SetActive(true);
                _backwardFlame.SetActive(false);
            }
            else if (translation < 0)
            {
                _forwardFlame.SetActive(false);
                _backwardFlame.SetActive(true);
            }
        }
        else
        {
            float actualTranslation = _model.CurrentSpeed;
            transform.Translate(0,actualTranslation,0);
            _forwardFlame.SetActive(false);
            _backwardFlame.SetActive(false);
        }
        
        float rotation = Input.GetAxis("Horizontal");
        if (rotation != 0 && _model.Devices["Engine"].IsActive)
        {
            float actualRotation = _model.GetActualRotation(rotation);
            transform.Rotate(0,0,actualRotation);
            if (rotation > 0)
            {
                _rightTurnFlame.SetActive(true);
                _leftTurnFlame.SetActive(false);
            }
            else if (rotation < 0)
            {
                _rightTurnFlame.SetActive(false);
                _leftTurnFlame.SetActive(true);
            }
        }
        else
        {
            _rightTurnFlame.SetActive(false);
            _leftTurnFlame.SetActive(false);
        }

        if (!_model.Devices["Engine"].IsActive || (translation == 0 && rotation == 0))
        {
            _model.DecayEngineHeat();
            _forwardFlame.SetActive(false);
            _backwardFlame.SetActive(false);
            _rightTurnFlame.SetActive(false);
            _leftTurnFlame.SetActive(false);
        }

        Vector2 newPosition = transform.position;
        _model.MovementVector = (newPosition - oldPosition).normalized;
        
        #endregion // Input handling
    }
}
