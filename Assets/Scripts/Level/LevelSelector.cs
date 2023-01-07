using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public List<LevelConfig> allLevels;
    public UnityEvent<LevelConfig> OnLevelSelect;

    private void Awake()
    {
        OnLevelSelect = new UnityEvent<LevelConfig>();
    }

    private void OnGUI()
    {
        for(int i = 0; i < allLevels.Count; i++)
        {
            if (GUILayout.Button($"Load Level {i}"))
            {
                Select(i);
            }
        }
    }

    public void Select(int id)
    {
        OnLevelSelect.Invoke(allLevels[id]);
    }

    private void OnDestroy()
    {
        OnLevelSelect.RemoveAllListeners();
    }


}
