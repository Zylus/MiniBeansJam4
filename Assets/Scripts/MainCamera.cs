using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private PlayerView _playerView;
    private Vector3 _offset = new Vector3(0,0,-10);

    public void Init()
    {
        _playerView = GameController.Instance.Player.View;
    }

    void LateUpdate()
    {
        if (_playerView != null)
        {
            SetCameraPosition();
        }
    }

    void SetCameraPosition()
    {
        transform.position = new Vector3
        (
            _playerView.transform.position.x + _offset.x,
            _playerView.transform.position.y + _offset.y,
            _offset.z
        );
    }
}
