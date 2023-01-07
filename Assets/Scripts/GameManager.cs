using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelLoader))]
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private LevelLoader levelLoader;
    private LevelSelector levelSelector;
    private Level loadedLevel;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private int MainMenuScene;
    [SerializeField]
    private int GameScene;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
        levelLoader = GetComponent<LevelLoader>();
        levelSelector = GetComponent<LevelSelector>();
    }

    private void Start()
    {
        levelSelector.OnLevelSelect.AddListener(HandleLevelSelection);
    }

    private void HandleLevelSelection(LevelConfig config)
    {
        levelSelector.enabled = false;
        levelLoader.LoadLevel(config);
        loadedLevel = new Level()
        {
            MaxPlayerCrop = config.MaxCombineStorage,
            MaxPlayerFuel = config.MaxFuel,

            CurrentPlayerCrop = 0,
            CurrentPlayerFuel = config.StartingFuel,
            CurrentTrailerCrop = 0
        };
        Player.instance.levelData = loadedLevel;
        UIManager.instance.OnLevelLoad(Player.instance, loadedLevel);
        Player.instance.OnMove.AddListener(OnPlayerMove);


        // Hide UI to select scene
    }

    private void OnPlayerMove(MovementControl onMove)
    {
        // Check for game over if out of fuel
    }

    private void OnGUI()
    {
        if (loadedLevel != null && GUILayout.Button("Unload Level"))
        {
            UnloadLevel();
        }
    }

    private void UnloadLevel()
    {
        levelSelector.enabled = true;
        loadedLevel = null;
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
        // Show UI to select scene 
    }
}
