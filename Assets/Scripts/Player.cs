using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    public List<MovementControl> controls;

    private MovementControl front;
    private MovementControl frontLeft;
    private MovementControl frontRight;
    private MovementControl back;
    private MovementControl backLeft;
    private MovementControl backRight;

    [SerializeField]
    private ParticleSystem depositFlow;
    public bool canRefuel { get; private set; }
    public bool canDeposit { get; private set; }

    [SerializeField] private Canvas canvas;
    private RectTransform canvasRect => canvas.GetComponent<RectTransform>();
    [SerializeField] private Button fuelButton;
    private RectTransform fuelButtonRect => fuelButton.GetComponent<RectTransform>();
    [SerializeField] private Button trailerButton;
    private RectTransform trailerButtonRect => trailerButton.GetComponent<RectTransform>();

    //public int CurrentFuel { get; private set; }
    //public int MaxFuel { get; private set; }
    //public int CurrentCrops { get; private set; }
    //public int MaxCrops { get; private set; }

    public Level levelData;

    private Vector3 frontRay => transform.position + transform.right + Vector3.up;
    private Vector3 frontLeftRay => transform.position + transform.right + transform.forward + Vector3.up;
    private Vector3 frontRightRay => transform.position + transform.right - transform.forward + Vector3.up;
    private Vector3 backRay => transform.position - transform.right + Vector3.up;
    private Vector3 backLeftRay => transform.position - transform.right + transform.forward + Vector3.up;
    private Vector3 backRightRay => transform.position - transform.right - transform.forward + Vector3.up;

    private Vector3 fuelStationRay => transform.position - transform.forward + Vector3.up * 3;
    private Vector3 trailerRay => transform.position + transform.forward + Vector3.up * 3;

    public UnityEvent<Crop> OnHarvest;
    public UnityEvent<MovementControl> OnMove;
    public UnityEvent OnRefuel;
    public UnityEvent<CropType> OnDeposit;

    public Animator animator;
    public SpriteRenderer currentCropIcon;

    public bool IsPerformingAction { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        instance = this;

        front = controls.First(x => x.direction == Direction.Front && x.rotation == Rotation.None);
        frontLeft = controls.First(x => x.direction == Direction.Front && x.rotation == Rotation.Left);
        frontRight = controls.First(x => x.direction == Direction.Front && x.rotation == Rotation.Right);
        back = controls.First(x => x.direction == Direction.Back && x.rotation == Rotation.None);
        backLeft = controls.First(x => x.direction == Direction.Back && x.rotation == Rotation.Left);
        backRight = controls.First(x => x.direction == Direction.Back && x.rotation == Rotation.Right);
        fuelButton.gameObject.SetActive(false);
        trailerButton.gameObject.SetActive(false);

        OnHarvest = new UnityEvent<Crop>();
        OnMove = new UnityEvent<MovementControl>();
        OnRefuel = new UnityEvent();
        OnDeposit = new UnityEvent<CropType>();

        fuelButton.onClick.AddListener(Refuel);
        trailerButton.onClick.AddListener(Deposit);

        IsPerformingAction = false;
        currentCropIcon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnHarvest.RemoveAllListeners();
        OnMove.RemoveAllListeners();
    }

    private void Start()
    {
        CheckNavigableControls();
        CheckForFuelStation();
        CheckForTrailer();
    }

    private void CheckNavigableControls()
    {
        // Front
        if (IsPerformingAction == false && TileIsNavigable(frontRay))
        {
            front.gameObject.SetActive(true);
            //frontLeft.gameObject.SetActive(true);
            //frontRight.gameObject.SetActive(true);
        }
        else
        {
            front.gameObject.SetActive(false);
            //frontLeft.gameObject.SetActive(false);
            //frontRight.gameObject.SetActive(false);
        }

        // Front Left if fron right cell is empty then allow turning left
        if (IsPerformingAction == false && TileIsNavigable(frontRay) && (TileIsNavigable(frontLeftRay) || TileIsNavigable(frontRightRay)))
        {
            frontLeft.gameObject.SetActive(true);
        }
        else
        {
            frontLeft.gameObject.SetActive(false);
        }

        // Front Right
        if (IsPerformingAction == false && TileIsNavigable(frontRay) && (TileIsNavigable(frontLeftRay) || TileIsNavigable(frontRightRay)))
        {
            frontRight.gameObject.SetActive(true);
        }
        else
        {
            frontRight.gameObject.SetActive(false);
        }

        // Back
        if (IsPerformingAction == false && TileIsNavigable(backRay))
        {
            back.gameObject.SetActive(true);
            //backLeft.gameObject.SetActive(true);
            //backRight.gameObject.SetActive(true);
        }
        else
        {
            back.gameObject.SetActive(false);
            //backLeft.gameObject.SetActive(false);
            //backRight.gameObject.SetActive(false);
        }

        // Back Left
        if (IsPerformingAction == false && TileIsNavigable(backRay) && (TileIsNavigable(backLeftRay) || TileIsNavigable(backRightRay)))
        {
            backLeft.gameObject.SetActive(true);
        }
        else
        {
            backLeft.gameObject.SetActive(false);
        }

        // Back Right
        if (IsPerformingAction == false && TileIsNavigable(backRay) && (TileIsNavigable(backLeftRay) || TileIsNavigable(backRightRay)))
        {
            backRight.gameObject.SetActive(true);
        }
        else
        {
            backRight.gameObject.SetActive(false);
        }
    }

    private void CheckForFuelStation()
    {
        var station = GetFuelStation(fuelStationRay);
        if (IsPerformingAction == false && station != null && levelData.CurrentPlayerFuel < levelData.MaxPlayerFuel)
        {
            Vector2 screenPoint = Camera.main.WorldToViewportPoint(station.UIAnchorPoint.position);
            fuelButtonRect.anchoredPosition3D = new Vector2(
                ((screenPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((screenPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

            fuelButton.gameObject.SetActive(true);
            canRefuel = true;
        }
        else
        {
            fuelButton.gameObject.SetActive(false);
            canRefuel = false;
        }
    }

    private void CheckForTrailer()
    {
        var trailer = GetTrailer(trailerRay);
        if (IsPerformingAction == false && trailer != null && levelData.CurrentPlayerCrop > 0 && (trailer.cropType == CropType.None || levelData.CurrentPlayerCropType == trailer.cropType))
        {
            Vector2 screenPoint = Camera.main.WorldToViewportPoint(trailer.UIAnchorPoint.position);
            trailerButtonRect.anchoredPosition3D = new Vector2(
                ((screenPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((screenPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

            trailerButton.gameObject.SetActive(true);
            canDeposit = true;
        }
        else
        {
            trailerButton.gameObject.SetActive(false);
            canDeposit = false;
        }
    }

    //Clicked Object
    private void Move(MovementControl control)
    {
        // ... or should I do this in game manager?
        if (levelData.CurrentPlayerFuel > 0)
        {
            levelData.CurrentPlayerFuel--;
        }
        else
            return;

        if (control.direction == Direction.Front)
        {
            var crop = GetTargetCrop(frontRay);
            if (crop != null)
                Harvest(crop);

            if (control.rotation == Rotation.Left)
            {
                PerformMove(control, "forward_left", transform.position + transform.right);
            }
            else if (control.rotation == Rotation.Right)
            {
                PerformMove(control, "forward_right", transform.position + transform.right);
            }
            else
            {
                PerformMove(control, "forward", transform.position + transform.right);
            }
        }
        else if (control.direction == Direction.Back)
        {
            //Don't allow harvesting if moving backwards
            //var crop = GetTargetCrop(backRay);
            //if (crop != null)
            //    Harvest(crop);

            if (control.rotation == Rotation.Left)
            {
                PerformMove(control, "back_left", transform.position - transform.right);
            }
            else if (control.rotation == Rotation.Right)
            {
                PerformMove(control, "back_right", transform.position - transform.right);
            }
            else
            {
                PerformMove(control, "back", transform.position - transform.right);
            }
        }
    }

    private void Update()
    {
        currentCropIcon.gameObject.transform.LookAt(Camera.main.transform);
        if (IsPerformingAction == false && Input.GetMouseButtonDown(0))
        {
            var control = GetHoveredControl();
            if (control != null)
                Move(control);
        }    
    }

    RaycastHit hit;
    private MovementControl GetHoveredControl()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Control")))
        {
            var comp = hit.collider.gameObject.GetComponent<MovementControl>();
            return comp;
        }

        return null;
    }

    private Crop GetTargetCrop(Vector3 rayPosition)
    {
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, 10f, LayerMask.GetMask("Crop")))
        {
            var comp = hit.collider.gameObject.GetComponentInParent<Crop>();
            return comp;
        }

        return null;
    }

    private FuelStation GetFuelStation(Vector3 rayPosition)
    {
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, 10f, LayerMask.GetMask("FuelStation")))
        {
            var comp = hit.collider.gameObject.GetComponentInParent<FuelStation>();
            return comp;
        }

        return null;
    }

    private Trailer GetTrailer(Vector3 rayPosition)
    {
        if (Physics.Raycast(rayPosition, Vector3.down, out hit, 10f, LayerMask.GetMask("Trailer")))
        {
            var comp = hit.collider.gameObject.GetComponentInParent<Trailer>();
            return comp;
        }

        return null;
    }

    private bool TileIsNavigable(Vector3 position)
    {
        return Physics.Raycast(position, Vector3.down, 10f, LayerMask.GetMask("Navigation"));
    }

    private void Harvest(Crop crop)
    {
        // Harvest only if there's enough space and if currently carried crop type matches
        if (levelData.CurrentPlayerCrop >= levelData.MaxPlayerCrop || (levelData.CurrentPlayerCropType != CropType.None && levelData.CurrentPlayerCropType != crop.cropType))
            return;
        levelData.CurrentPlayerCrop++;
        levelData.CurrentPlayerCropType = crop.cropType;

        var sprite = crop.getIconSprite();
        if (sprite != null)
        {
            currentCropIcon.gameObject.SetActive(true);
            currentCropIcon.sprite = crop.getIconSprite();
        }
        else 
        { 
            currentCropIcon.gameObject.SetActive(false);
        }


        crop.Harvest();
        // Play harvest anim
        OnHarvest.Invoke(crop);
    }

    private void Refuel()
    {
        levelData.CurrentPlayerFuel = levelData.MaxPlayerFuel;
        fuelButton.gameObject.SetActive(false);
        canRefuel = false;
        OnRefuel.Invoke();
    }

    private void Deposit()
    {
        var objective = levelData.depositObjectives.First(x => x.cropType == levelData.CurrentPlayerCropType);
        trailerButton.gameObject.SetActive(false);
        canDeposit = false;
        StartCoroutine(PerformDeposit(objective, 1.3f));
    }

    UnityEvent moveEvent;

    private void PerformMove(MovementControl control, string trigger, Vector3 endPosition)
    {
        HandleActionStart();
        animator.SetTrigger(trigger);

        moveEvent = new UnityEvent();
        Anim_OnMoveEnd();

        moveEvent.AddListener(() =>
        {
            transform.position = endPosition; // ensure fine position is correct
            HandleActionEnd();
            OnMove.Invoke(control);
        });
    }

    private IEnumerator PerformDeposit(DepositObjective objective, float duration)
    {
        HandleActionStart();
        var depositCropType = levelData.CurrentPlayerCropType;
        currentCropIcon.gameObject.SetActive(false);

        var t = 0f;
        objective.currentStorage += levelData.CurrentPlayerCrop;

        levelData.CurrentPlayerCrop = 0;
        levelData.CurrentPlayerCropType = CropType.None;

        var trailerObject = GetTrailer(trailerRay);
        var color = trailerObject.Mapping.First(x => x.cropType == depositCropType).material.color;
        depositFlow.Play();
        var mainModule = depositFlow.main;
        mainModule.startColor = color;

        while (t < duration/2f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        
        trailerObject.cropType = depositCropType;
        trailerObject.HandleFill(depositCropType);
        depositFlow.Stop();
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        OnDeposit.Invoke(objective.cropType);
        HandleActionEnd();
    }

    private void HandleActionStart()
    {
        IsPerformingAction = true;
        CheckNavigableControls();
        CheckForFuelStation();
        CheckForTrailer();
    }

    private void HandleActionEnd()
    {
        IsPerformingAction = false;
        CheckNavigableControls();
        CheckForFuelStation();
        CheckForTrailer();
    }


    public void Anim_OnMoveEnd()
    {
        moveEvent.Invoke();
    }
}
