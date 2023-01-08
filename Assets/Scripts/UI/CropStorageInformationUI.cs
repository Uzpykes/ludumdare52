using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CropStorageInformationUI : InformationController
{
    public CropType currentCropType;
    public Image CropImage;
    public List<CropTypeToSprite> typeToSpriteMapping;

    public void Init(int currentVal, int maxVal, CropType type)
    {
        base.Init(currentVal, maxVal);
        currentCropType = type;
        if (type == CropType.None)
            CropImage.gameObject.SetActive(false);
        else
        {
            CropImage.sprite = typeToSpriteMapping.First(x => x.cropType == currentCropType).sprite;
            CropImage.gameObject.SetActive(true);
        }
    }

    public void OnUpdate(int currentVal, CropType type)
    {
        base.OnUpdate(currentVal);
        currentCropType = type;
        if (type == CropType.None)
            CropImage.gameObject.SetActive(false);
        else
        {
            CropImage.sprite = typeToSpriteMapping.First(x => x.cropType == currentCropType).sprite;
            CropImage.gameObject.SetActive(true);
        }
    }
}
