using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int StepsTaken;

    public int MaxPlayerFuel;
    public int MaxPlayerCrop;

    public int CurrentPlayerFuel;
    public int CurrentPlayerCrop;
    public CropType CurrentPlayerCropType;

    public List<DepositObjective> depositObjectives = new List<DepositObjective>();

}

public class DepositObjective
{
    public CropType cropType;
    public int currentStorage;
    public int maxStorage;
}