using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelgroup", menuName = "levelgroup")]
public class LevelGroup : ScriptableObject
{
    public string Title;
    public List<LevelConfig> Levels;
}
