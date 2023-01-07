using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private InformationController fuelInfo;
    [SerializeField]
    private InformationController combineStorageInfo;
    [SerializeField]
    private InformationController trailerStorageInfo;

    private Player player;
    private Level levelInfo;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // When level is loaded
    public void OnLevelLoad(Player player, Level levelInfo)
    {
        this.player = player;
        this.levelInfo = levelInfo;

        fuelInfo.Init(levelInfo.CurrentPlayerFuel, levelInfo.MaxPlayerFuel);
        combineStorageInfo.Init(levelInfo.CurrentPlayerCrop, levelInfo.MaxPlayerCrop);
        trailerStorageInfo.Init(levelInfo.CurrentTrailerCrop, levelInfo.MaxTrailerCrop);
        player.OnMove.AddListener(HandleMove);
        player.OnHarvest.AddListener(HandleHarvest);
    }

    // When level becomes playable
    public void OnLevelStart()
    {

    }

    // When level ends (all crops are harvested and deposited)
    public void OnLevelEnd()
    {
        this.player.OnMove.RemoveListener(HandleMove);
        this.player.OnHarvest.RemoveListener(HandleHarvest);
    }

    // When level is unloaded
    public void OnLevelUnload()
    {
    }

    private void HandleMove(MovementControl control)
    {
        fuelInfo.OnUpdate(levelInfo.CurrentPlayerFuel);
    }

    private void HandleHarvest(Crop crop)
    {
        combineStorageInfo.OnUpdate(levelInfo.CurrentPlayerCrop);
        trailerStorageInfo.OnUpdate(levelInfo.CurrentTrailerCrop);
    }

}
