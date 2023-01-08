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

    private LevelConfig loadedLevelConfig;
    private Level loadedLevel;

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
        UIManager.instance.LevelEndUI.OnRestart.AddListener(HandleRestart);
        UIManager.instance.LevelEndUI.OnNextLevel.AddListener(HandleNextLevel);
        UIManager.instance.LevelEndUI.OnReturnToLevelSelect.AddListener(HandleReturnToLevelSelect);
        UIManager.instance.LevelEndUI.OnReturnToMainMenu.AddListener(HandleReturnToMainMenu);
        levelSelector.OnLevelSelect.AddListener(HandleLevelSelection);
    }

    private void HandleRestart()
    {
        //UnloadLevel();
        HandleLevelSelection(loadedLevelConfig);
    }

    private void HandleNextLevel()
    {
        //var next = levelSelector.GetNextConfig(loadedLevelConfig);
        ////UnloadLevel();
        //HandleLevelSelection(next);
    }

    private void HandleReturnToLevelSelect()
    {
        UIManager.instance.ShowLevelList();
    }

    private void HandleReturnToMainMenu()
    {
        UIManager.instance.ShowMainMenu();
    }

    private void HandleLevelSelection(LevelConfig config)
    {
        loadedLevelConfig = config;
        levelLoader.LoadLevel(config);
        loadedLevel = CreateLevel(config);

        Player.instance.levelData = loadedLevel;
        UIManager.instance.OnLevelLoad(Player.instance, loadedLevel);
        Player.instance.OnMove.AddListener(OnPlayerMove);
        Player.instance.OnDeposit.AddListener(OnPlayerDeposit);


        // Hide UI to select scene
    }

    private void OnPlayerMove(MovementControl onMove)
    {
        loadedLevel.StepsTaken++;
        CheckWinOrFail();
    }

    private void OnPlayerDeposit(CropType cropType)
    {
        CheckWinOrFail();
    }

    private void UnloadLevel()
    {
        levelSelector.enabled = true;
        loadedLevel = null;
        SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
        UIManager.instance.OnLevelUnload();
    }

    private void CheckWinOrFail()
    {
        if (IsWinCondition())
        {
            OnWin();
        }
        else if (IsFailCondition())
        {
            OnFail();
        }
    }

    private bool IsWinCondition()
    {
        foreach (var trailer in loadedLevel.depositObjectives)
        {
            if (trailer.currentStorage < trailer.maxStorage)
                return false;
        }

        return true;
    }

    private bool IsFailCondition()
    {
        if (loadedLevel.CurrentPlayerFuel == 0)
        {
            if (Player.instance.canRefuel == false && Player.instance.canDeposit == false)
            {
                foreach (var trailer in loadedLevel.depositObjectives)
                {
                    if (trailer.maxStorage > trailer.currentStorage)
                        return true;
                }
            }
        }

        return false;

        // if out of fuel and cannot refuel or deposit - loose
    }

    public void OnWin()
    {
        UIManager.instance.ShowWinScreen();
        UnloadLevel();
    }

    public void OnFail()
    {
        UIManager.instance.ShowFailScreen();
        UnloadLevel();
    }

    private Level CreateLevel(LevelConfig config)
    {
        var l = new Level()
        {
            StepsTaken = 0,

            MaxPlayerCrop = config.MaxCombineStorage,
            MaxPlayerFuel = config.MaxFuel,

            CurrentPlayerCrop = 0,
            CurrentPlayerFuel = config.StartingFuel
        };

        foreach (var cropQuantity in config.CropQuantities)
        {
            l.depositObjectives.Add(new DepositObjective()
            {
                cropType = cropQuantity.cropType,
                currentStorage = 0,
                maxStorage = cropQuantity.quantity,
            });
        }

        return l;
    }
}
