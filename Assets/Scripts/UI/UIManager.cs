using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public RectTransform InformationPanel;
    public RectTransform ObjectivesPanel;

    [SerializeField]
    private FuelInformationUI fuelInfo;
    [SerializeField]
    private CropStorageInformationUI combineStorageInfo;
    // This is prepopulated with disabled objects and needed quanity is enabled when level is loaded
    [SerializeField]
    private List<CropStorageInformationUI> trailerStorageInfos;

    public LevelSelectorUI LevelSelectorUI;
    public LevelEndUI LevelEndUI;

    private Player player;
    private Level levelInfo;

    public RectTransform MainMenu;
    public Button Play;
    public Button Restart;
    public Button SelectLevel;
    public Button Quit;


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

    private void Start()
    {
        Play.onClick.AddListener(OnPlay);
        Quit.onClick.AddListener(() => Application.Quit());
    }


    // When level is loaded
    public void OnLevelLoad(Player player, Level levelInfo)
    {
        this.player = player;
        this.levelInfo = levelInfo;

        fuelInfo.Init(levelInfo.CurrentPlayerFuel, levelInfo.MaxPlayerFuel);
        combineStorageInfo.Init(levelInfo.CurrentPlayerCrop, levelInfo.MaxPlayerCrop, levelInfo.CurrentPlayerCropType);
        for (int i = 0; i < levelInfo.depositObjectives.Count; i++)
        {
            trailerStorageInfos[i].gameObject.SetActive(true);
            trailerStorageInfos[i].Init(levelInfo.depositObjectives[i].currentStorage, levelInfo.depositObjectives[i].maxStorage, levelInfo.depositObjectives[i].cropType);
        }

        player.OnMove.AddListener(HandleMove);
        player.OnHarvest.AddListener(HandleHarvest);
        player.OnRefuel.AddListener(HandleRefuel);
        player.OnDeposit.AddListener(HandleDeposit);

        LevelSelectorUI.gameObject.SetActive(false);
        InformationPanel.gameObject.SetActive(true);
        ObjectivesPanel.gameObject.SetActive(true);
    }

    // When level becomes playable
    public void OnLevelStart()
    {

    }

    // When level ends (all crops are harvested and deposited)
    public void OnLevelEnd()
    {
        InformationPanel.gameObject.SetActive(true);
        ObjectivesPanel.gameObject.SetActive(true);
        this.player.OnMove.RemoveListener(HandleMove);
        this.player.OnHarvest.RemoveListener(HandleHarvest);
    }

    // When level is unloaded
    public void OnLevelUnload()
    {
        foreach(var info in trailerStorageInfos)
        {
            info.gameObject.SetActive(false);
        }
    }

    private void HandleMove(MovementControl control)
    {
        fuelInfo.OnUpdate(levelInfo.CurrentPlayerFuel);
    }

    private void HandleHarvest(Crop crop)
    {
        combineStorageInfo.OnUpdate(levelInfo.CurrentPlayerCrop, crop.cropType);
        trailerStorageInfos.First(x => x.currentCropType == crop.cropType).OnUpdate(levelInfo.depositObjectives.First(x => x.cropType == crop.cropType).currentStorage);
    }

    private void HandleRefuel()
    {
        fuelInfo.OnUpdate(levelInfo.CurrentPlayerFuel);
    }

    private void HandleDeposit(CropType type)
    {
        combineStorageInfo.OnUpdate(levelInfo.CurrentPlayerCrop, levelInfo.CurrentPlayerCropType);
        trailerStorageInfos.First(x => x.currentCropType == type).OnUpdate(levelInfo.depositObjectives.First(x => x.cropType == type).currentStorage);
    }

    public void ShowWinScreen()
    {
        LevelEndUI.ShowWinUI();
    }

    public void ShowFailScreen()
    {
        LevelEndUI.ShowFailUI();
    }

    public void OnPlay()
    {
        LevelSelectorUI.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false);
    }

    public void ShowMainMenu()
    {
        LevelSelectorUI.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
    }

    public void ShowLevelList()
    {
        LevelSelectorUI.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false);
    }
}
