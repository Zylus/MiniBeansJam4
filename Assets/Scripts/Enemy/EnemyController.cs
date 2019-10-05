using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController
{
    public EnemyView View { get; set; }
    public Enemy Model { get; set; }

    public void CreateEnemy(Vector2 position)
    {
        GameObject viewObjPrefab = Resources.Load<GameObject>("EnemyShip");
        GameObject viewObj = GameObject.Instantiate(viewObjPrefab, position, Quaternion.identity);
        Enemy model = new Enemy();
        model.Init();
        EnemyView view = viewObj.GetComponent<EnemyView>();
        view.Init(model);
        this.View = view;
        this.Model = model;
    }
}
