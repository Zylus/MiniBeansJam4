using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    public PlayerView View { get; set; }
    public Player Model { get; set; }

    public void CreatePlayer()
    {
        GameObject viewObjPrefab = Resources.Load<GameObject>("PlayerShip");
        GameObject viewObj = GameObject.Instantiate(viewObjPrefab, new Vector2(0,0), Quaternion.identity);
        Player model = new Player();
        model.Init();
        PlayerView view = viewObj.GetComponent<PlayerView>();
        view.Init(model);
        this.View = view;
        this.Model = model;
    }
}
