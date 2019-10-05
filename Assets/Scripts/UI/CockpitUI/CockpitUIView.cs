﻿using System;
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
}