using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelSelectorUI : MonoBehaviour
{
    public RectTransform GroupListParent;

    public LevelGroupUI LevelGroupPrefab;
    public LevelPanelUI LevelPanelPrefab;
    public LevelSmallObjectiveUI SmallObjectivePrefab;

    // Should be global somewhere...
    public List<CropTypeToSprite> cropTypeToSprites;

    public UnityEvent<int> OnLevelSelect;

    private void Awake()
    {
        OnLevelSelect = new UnityEvent<int>();
    }


    public void CreateLevelList(List<LevelGroup> levelGroups)
    {
        int currentId = 0;
        foreach (var group in levelGroups)
        {
            var groupUi = Instantiate(LevelGroupPrefab, GroupListParent);
            groupUi.GroupTitle.text = group.Title;

            foreach (var level in group.Levels)
            {
                var levelUi = Instantiate(LevelPanelPrefab, groupUi.LevelListParent);
                levelUi.Title.text = level.DisplayName;
                if (levelUi.Objectives == null)
                    levelUi.Objectives = new List<LevelSmallObjectiveUI>();
                foreach (var objective in level.CropQuantities)
                {
                    var objectiveUi = Instantiate(SmallObjectivePrefab, levelUi.LevelObjectivesParent);
                    objectiveUi.ObjectiveImage.sprite = cropTypeToSprites.First(x => x.cropType == objective.cropType).sprite;
                    objectiveUi.ObjectiveCount.text = objective.quantity.ToString();
                }
                levelUi.ConfigId = currentId;
                levelUi.StartButton.onClick.AddListener(() => { OnLevelSelect.Invoke(levelUi.ConfigId);  });
                currentId++;
            }
        }

    }
}
