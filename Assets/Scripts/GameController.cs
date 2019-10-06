using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    #region Singleton

    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    #endregion // singleton

    [SerializeField] private GameObject _cameraObj;
    [SerializeField] private int _enemyCount;
    [SerializeField] private GameObject _stationsParent;

    public PlayerController Player { get; set; }
    public MainCamera MainCamera { get; set; }
    public List<EnemyController> Enemies { get; set; }
    public GameObject PlayerGhostPrefab { get; set; }
    public List<Station> Stations { get; set; }

    private void Start()
    {
        Player = new PlayerController();
        Player.CreatePlayer();
        MainCamera = _cameraObj.GetComponent<MainCamera>();
        MainCamera.Init();
        CreateEnemies();
        InitializeUI();
        InitializeDialogue();
        UIController.Instance.InitializeCockpitUI();
        PlayerGhostPrefab = Resources.Load<GameObject>("PlayerGhost");
        Stations = new List<Station>();
        foreach(Transform child in _stationsParent.transform)
        {
            Stations.Add(child.gameObject.GetComponent<Station>());
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            RestartGame();
        }

        if (Player.Model.ActiveMission != null)
        {
            float timeFactor = 1f;
            float reducedReward = Player.Model.ActiveMission.Reward;
            reducedReward = reducedReward - (Time.deltaTime * timeFactor);
            if (reducedReward < Mission.MIN_REWARD)
            {
                reducedReward = Mission.MIN_REWARD;
            }

            Player.Model.ActiveMission.Reward = reducedReward;
            UIController.Instance.CockpitUI.View.OnMissionReceived(Player.Model.ActiveMission);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    public Vector2 GetTargetVector()
    {
        if (Player.Model.ActiveMission == null)
        {
            return Vector2.zero;
        }

        Vector2 targetDirection = Player.Model.ActiveMission.EndStation.transform.position - Player.View.transform.position;
        targetDirection = targetDirection.normalized;
        return targetDirection;
    }

    public void OnStationEntered(Station station)
    {
        if (Player.Model.ActiveMission == null)
        {
            Mission newMission = new Mission();
            newMission.Init(station);
            Player.Model.ActiveMission = newMission;
            UIController.Instance.CockpitUI.View.OnMissionReceived(newMission);
        }
        else
        {
            if (station == Player.Model.ActiveMission.EndStation)
            {
                Player.Model.Cash += (int)Mathf.Round(Player.Model.ActiveMission.Reward);
                UIController.Instance.CockpitUI.View.UpdateCashText(Player.Model.Cash);
                Mission newMission = new Mission();
                newMission.Init(station);
                Player.Model.ActiveMission = newMission;
                UIController.Instance.CockpitUI.View.OnMissionReceived(newMission);
            }
        }
    }

    public void JettisonCargo()
    {
        if (Player.Model.ActiveMission != null)
        {
            Player.Model.ActiveMission = null;
            UIController.Instance.CockpitUI.View.OnMissionReceived(null);
        }
    }

    public void PunishPlayer()
    {
        int fine = (int)(300f * Random.Range(0.8f, 1.2f));
        Player.Model.Cash -= fine;
        UIController.Instance.CockpitUI.View.UpdateCashText(Player.Model.Cash);
    }

    private void CreateEnemies()
    {
        Enemies = new List<EnemyController>();
        for (int i = 0; i < _enemyCount; i++)
        {
            EnemyController enemyController = new EnemyController();
            Vector2 position = Player.View.transform.position;
            Vector2 offset = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            enemyController.CreateEnemy(position + offset);
            Enemies.Add(enemyController);
        }
    }

    private void InitializeUI()
    {
        GameObject uiCtrPrefab = Resources.Load<GameObject>("UIController");
        GameObject.Instantiate(uiCtrPrefab, new Vector2(0,0), Quaternion.identity);
    }

    private void InitializeDialogue()
    {
        GameObject dlgCtrPrefab = Resources.Load<GameObject>("DialogueController");
        GameObject.Instantiate(dlgCtrPrefab, new Vector2(0,0), Quaternion.identity);
    }
}
