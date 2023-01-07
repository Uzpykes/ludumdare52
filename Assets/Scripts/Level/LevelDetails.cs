using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "level")]
public class LevelConfig : ScriptableObject
{
    public int MaxFuel;
    public int StartingFuel;
    public int MaxCombineStorage;

    // Each level layout is saved as a texture
    // First 8x8 area is ground layer, second 8x8 area is object layer
    public Texture2D LevelTexture;

    public List<ColorToObjectMap> ObjectMapping;

    private void OnValidate()
    {
        ValidateMapping();
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

}

[System.Serializable]
public struct ColorToObjectMap
{
    public Color color;
    public GameObject gameObject;
}