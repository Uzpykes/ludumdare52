using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEndUI : MonoBehaviour
{
    [Header("Level Win")]
    public GameObject LevelWinScreen;
    public Button NextLevelButton;

    [Header("Level Fail")]
    public GameObject LevelFailScreen;
    public Button RestartButton;
    public Button LevelSelectorButton;
    public Button MainMenuButton;

    public LevelConfig nextLevel;
    public UnityEvent OnNextLevel;
    public UnityEvent OnRestart;
    public UnityEvent OnReturnToLevelSelect;
    public UnityEvent OnReturnToMainMenu;

    private void Awake()
    {
        OnNextLevel = new UnityEvent();
        OnRestart = new UnityEvent();
        OnReturnToLevelSelect = new UnityEvent();
        OnReturnToMainMenu = new UnityEvent();
    }

    private void Start()
    {
        NextLevelButton.onClick.AddListener(OnNextLevel.Invoke);
        NextLevelButton.onClick.AddListener(Hide);
        RestartButton.onClick.AddListener(OnRestart.Invoke);
        RestartButton.onClick.AddListener(Hide);
        LevelSelectorButton.onClick.AddListener(OnReturnToLevelSelect.Invoke);
        LevelSelectorButton.onClick.AddListener(Hide);
        MainMenuButton.onClick.AddListener(OnReturnToMainMenu.Invoke);
        MainMenuButton.onClick.AddListener(Hide);
    }

    public void ShowWinUI()
    {
        LevelWinScreen.SetActive(true);
    }

    public void ShowFailUI()
    {
        LevelFailScreen.SetActive(true);
    }

    public void Hide()
    {
        LevelWinScreen.SetActive(false);
        LevelFailScreen.SetActive(false);
    }
}
