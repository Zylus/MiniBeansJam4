using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CockpitUIView : MonoBehaviour
{
    private CockpitUI _model;
    [SerializeField] private Slider _engineThrustSlider;
    [SerializeField] private Slider _heatEmissionSlider;
    [SerializeField] private Slider _electroEmissionSlider;
    [SerializeField] private Button _toggleViewportButton;
    [SerializeField] private Button _toggleEngineButton;
    [SerializeField] private GameObject _viewportDisabledObject;
    [SerializeField] private GameObject _engineActiveLamp;
    [SerializeField] private GameObject _engineInactiveLamp;
    [SerializeField] private Text _messageText;
    [SerializeField] private Text _missionText;
    [SerializeField] private Text _cashText;

    public event EventHandler OnViewportToggledEvent;
    public event EventHandler OnEngineToggledEvent;

    public void Init(CockpitUI model)
    {
        _model = model;
        _toggleViewportButton.onClick.AddListener(OnViewportToggled);
        _toggleEngineButton.onClick.AddListener(OnEngineToggled);
    }

    private void LateUpdate()
    {
        _engineThrustSlider.value = _model.Momentum;
        _heatEmissionSlider.value = _model.HeatEmissions;
        _electroEmissionSlider.value = _model.ElectroEmissions;
    }

    private void OnViewportToggled()
    {
        _viewportDisabledObject.SetActive(!_viewportDisabledObject.activeSelf);

        EventHandler handler = OnViewportToggledEvent;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    private void OnEngineToggled()
    {
        _engineActiveLamp.SetActive(!_engineActiveLamp.activeSelf);
        _engineInactiveLamp.SetActive(!_engineInactiveLamp.activeSelf);

        EventHandler handler = OnEngineToggledEvent;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    public void OnMessageReceived(object sender, MessageSendingEventArgs e)
    {
        string message = DialogueController.Instance.GetMessage(e.type);
        if (string.IsNullOrEmpty(message))
        {
            _messageText.gameObject.SetActive(false);
        }
        else
        {
            _messageText.text = message;
            _messageText.gameObject.SetActive(true);
        }
    }

    public void OnMissionReceived(Mission mission)
    {
        _missionText.text = "Current Mission:\n\nTarget: " + mission.EndStation.name + "\nReward: " + mission.Reward.ToString() + "$\nCargo: " + mission.CargoName;
    }

    public void UpdateCashText(int cash)
    {
        _cashText.text = cash.ToString() + " $";
    }
}
