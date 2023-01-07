using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InformationController : MonoBehaviour
{
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
