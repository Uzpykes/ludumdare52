using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public int Value = 1;

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


}
