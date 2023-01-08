using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trailer : MonoBehaviour
{
    // Do I really need this?
    public static List<Trailer> Trailers;

    public CropType cropType;
    public Transform UIAnchorPoint;

    public List<CropTypeToMaterialMapping> Mapping;
    public MeshRenderer wheatLevel;
    public SpriteRenderer icon;

    private void OnEnable()
    {
        if (Trailers == null)
            Trailers = new List<Trailer>();

        Trailers.Add(this);
    }

    private void Start()
    {
        icon.gameObject.transform.LookAt(Camera.main.gameObject.transform);
        icon.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Trailers.Remove(this);
    }

    public void HandleFill(CropType cropType)
    {
        wheatLevel.gameObject.SetActive(true);
        wheatLevel.material = Mapping.First(x => x.cropType == cropType).material;
        icon.gameObject.SetActive(true);
        icon.sprite = Mapping.First(x => x.cropType == cropType).sprite;
    }

}

[System.Serializable]
public struct CropTypeToMaterialMapping
{
    public CropType cropType;
    public Material material;
    public Sprite sprite;
}