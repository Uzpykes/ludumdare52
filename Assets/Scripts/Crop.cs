using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropType cropType;
    public int value = 1;
    public SpriteRenderer icon;

    public void Harvest()
    {
        StartCoroutine(HarvestAnim());
    }

    private IEnumerator HarvestAnim()
    {
        float t = 0;

        while (t < .5f)
        {
            t += Time.deltaTime;
            gameObject.transform.position = gameObject.transform.position - (Vector3.down * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public Sprite getIconSprite()
    {
        return icon.sprite;
    }

}

public enum CropType
{
    None,
    Wheat,
    Barley
}
