using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(LevelConfig configuration)
    {
        var texture = configuration.LevelTexture;
        var levelHeight = texture.height;
        var levelWidth = texture.width / 3; // Level is always the size of the texture;


        // Using color as key directly doesn't work :( 
        Dictionary<string, GameObject> mapping = configuration.ObjectMapping.ToDictionary(x => x.color.ToString(), y => y.gameObject);

        for (int x = 0; x < texture.width * 2 / 3; x++) // ignore last third of width as that part is taken up by navigation texture
        { 
            for (int y = 0; y < texture.height; y++)
            {
                var color = texture.GetPixel(x, y);
                if (color.a < .5f)
                    continue;

                var go = mapping[color.ToString()];
                if (go == null)
                    continue;

                var instance = Instantiate(go, new Vector3(x % levelWidth, 0, y), go.transform.rotation);
            }
        }
    }
}
