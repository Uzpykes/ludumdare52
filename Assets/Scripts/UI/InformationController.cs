using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationController : MonoBehaviour
{
    public Image MainImage;
    public TextMeshProUGUI CurrentValueField;
    public TextMeshProUGUI MaxValueField;
    public TextMeshProUGUI SeparatorField;

    public void Init(int currentVal, int maxVal)
    {
        CurrentValueField.text = currentVal.ToString();
        MaxValueField.text = maxVal.ToString();
    }

    public void OnUpdate(int currentVal)
    {
        CurrentValueField.text = currentVal.ToString();
    }
}

[System.Serializable]
public struct CropTypeToSprite
{
    public CropType cropType;
    public Sprite sprite;
}
