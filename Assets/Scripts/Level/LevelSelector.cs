using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class LevelSelector : MonoBehaviour
{
    public List<LevelGroup> levels;
    public UnityEvent<LevelConfig> OnLevelSelect;

    private int lastIndex;


    private void Awake()
    {
        OnLevelSelect = new UnityEvent<LevelConfig>();
    }

    private void Start()
    {
        UIManager.instance.LevelSelectorUI.CreateLevelList(levels);
        UIManager.instance.LevelSelectorUI.OnLevelSelect.AddListener(Select);
        UIManager.instance.LevelEndUI.OnNextLevel.AddListener(SelectNext);
    }

    public void Select(int id)
    {
        lastIndex = id;
        var config = GetConfig(id);

        var nextLevel = GetConfig(id + 1);
        if (nextLevel == null)
            UIManager.instance.LevelEndUI.NextLevelButton.interactable = false;
        else
            UIManager.instance.LevelEndUI.NextLevelButton.interactable = true;

        OnLevelSelect.Invoke(config);
    }

    public void SelectNext()
    {
        Select(lastIndex + 1);
    }


    private void OnDestroy()
    {
        OnLevelSelect.RemoveAllListeners();
    }

    private LevelConfig GetConfig(int id)
    {
        var index = 0;
        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < levels[i].Levels.Count; j++)
            {
                if (index == id)
                    return levels[i].Levels[j];
                index++;
            }
        }
        return null;
    }

}
