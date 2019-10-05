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

    public PlayerController Player { get; set; }
    public MainCamera MainCamera { get; set; }
    public List<EnemyController> Enemies { get; set; }
    public GameObject PlayerGhostPrefab { get; set; }

    private void Start()
    {
        Player = new PlayerController();
        Player.CreatePlayer();
        MainCamera = _cameraObj.GetComponent<MainCamera>();
        MainCamera.Init();
        CreateEnemies();
        InitializeUI();
        UIController.Instance.InitializeCockpitUI();
        PlayerGhostPrefab = Resources.Load<GameObject>("PlayerGhost");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void CreateEnemies()
    {
        Enemies = new List<EnemyController>();
        for (int i = 0; i < _enemyCount; i++)
        {
            EnemyController enemyController = new EnemyController();
            Vector2 position = Player.View.transform.position;
            Vector2 offset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            enemyController.CreateEnemy(position + offset);
            Enemies.Add(enemyController);
        }
    }

    private void InitializeUI()
    {
        GameObject uiCtrPrefab = Resources.Load<GameObject>("UIController");
        GameObject.Instantiate(uiCtrPrefab, new Vector2(0,0), Quaternion.identity);
    }
}
