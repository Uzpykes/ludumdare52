using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "level")]
public class LevelConfig : ScriptableObject
{
    public string DisplayName;
    public string Description;

    public int MaxFuel;
    public int StartingFuel;
    public int MaxCombineStorage;

    // Each level layout is saved as a texture
    // First 8x8 area is ground layer, second 8x8 area is object layer
    public Texture2D LevelTexture;

    public List<CropInfo> CropQuantities;
    public List<ColorToObjectMap> ObjectMapping;


    private void OnValidate()
    {
        ValidateMapping();
        ValidateCropQuantities();
    }

    private void ValidateMapping()
    {
        if (LevelTexture == null)
            return;

        if (ObjectMapping == null)
            ObjectMapping = new List<ColorToObjectMap>();

        var colors = LevelTexture.GetPixels().Distinct().ToList();

        foreach(var color in colors)
        {
            if (color.a < .5f)
                continue;

            if (!ObjectMapping.Any(x => x.color == color))
            {
                ObjectMapping.Add(new ColorToObjectMap()
                {
                    color = color,
                    gameObject = null
                });
            }
        }

        ObjectMapping.OrderBy(x => x.color);
    }   

    private void ValidateCropQuantities()
    {
        if (LevelTexture == null)
            return;

        if (CropQuantities == null)
            CropQuantities = new List<CropInfo>();

        foreach(var objMap in ObjectMapping)
        {
            if (objMap.gameObject != null && objMap.gameObject.TryGetComponent<Crop>(out Crop crop))
            {
                if (CropQuantities.Any(x => x.cropType == crop.cropType))
                    continue;

                var cropInfo = new CropInfo()
                {
                    cropType = crop.cropType,
                    quantity = 0
                };

                CropQuantities.Add(cropInfo);
            }
        }

        CropQuantities.OrderBy(x => x.cropType);
    }

}

[System.Serializable]
public struct ColorToObjectMap
{
    public Color color;
    public GameObject gameObject;
}

[System.Serializable]
public struct CropInfo
{
    public CropType cropType;
    public int quantity;
}