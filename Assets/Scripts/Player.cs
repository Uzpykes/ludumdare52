using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    [SerializeField] private Canvas canvas;
    private RectTransform canvasRect => canvas.GetComponent<RectTransform>();
    [SerializeField] private Button fuelButton;
    private RectTransform fuelButtonRect => canvas.GetComponent<RectTransform>();

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

    public UnityEvent<Crop> OnHarvest;
    public UnityEvent<MovementControl> OnMove;

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

        OnHarvest = new UnityEvent<Crop>();
        OnMove = new UnityEvent<MovementControl>();
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

    //public void Init(int maxFuel, int currentFuel, int maxStorage)
    //{
    //    MaxFuel = maxFuel;
    //    CurrentFuel = currentFuel;
    //    MaxCrops = maxStorage;
    //    CurrentCrops = 0;
    //}

    // Sends multiple raycasts around to check if any of them intersect navigable tiles
    private void CheckNavigableControls()
    {
        // Front
        if (TileIsNavigable(frontRay))
        {
            front.gameObject.SetActive(true);
        }
        else
        {
            front.gameObject.SetActive(false);
        }

        // Front Left
        if (TileIsNavigable(frontLeftRay))
        {
            frontLeft.gameObject.SetActive(true);
        }
        else
        {
            frontLeft.gameObject.SetActive(false);
        }

        // Front Right
        if (TileIsNavigable(frontRightRay))
        {
            frontRight.gameObject.SetActive(true);
        }
        else
        {
            frontRight.gameObject.SetActive(false);
        }

        // Back
        if (TileIsNavigable(backRay))
        {
            back.gameObject.SetActive(true);
        }
        else
        {
            back.gameObject.SetActive(false);
        }

        // Back Left
        if (TileIsNavigable(backLeftRay))
        {
            backLeft.gameObject.SetActive(true);
        }
        else
        {
            backLeft.gameObject.SetActive(false);
        }

        // Back Right
        if (TileIsNavigable(backRightRay))
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
        if (station != null)
        {
            fuelButton.gameObject.SetActive(true);
            //Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, station.transform.position);
            //fuelButtonRect.anchoredPosition = screenPoint - canvasRect.sizeDelta / 2f; ;

            var screenPoint = Camera.main.WorldToScreenPoint(station.transform.position);
            Debug.Log(screenPoint);
            fuelButtonRect.anchoredPosition = screenPoint;

        }
        else
            fuelButton.gameObject.SetActive(false);
    }

    private void CheckForTrailer()
    {

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

        OnMove.Invoke(control);
        if (control.direction == Direction.Front)
        {
            var crop = GetTargetCrop(frontRay);
            if (crop != null)
                Harvest(crop);

            gameObject.transform.position = gameObject.transform.position + transform.right;
            if (control.rotation == Rotation.Left)
            {
                gameObject.transform.Rotate(Vector3.up, -90);
            }
            if (control.rotation == Rotation.Right)
            {
                gameObject.transform.Rotate(Vector3.up, 90);
            }
        }
        else if (control.direction == Direction.Back)
        {
            var crop = GetTargetCrop(backRay);
            if (crop != null)
                Harvest(crop);

            gameObject.transform.position = gameObject.transform.position - transform.right;
            if (control.rotation == Rotation.Left)
            {
                gameObject.transform.Rotate(Vector3.up, 90);
            }
            if (control.rotation == Rotation.Right)
            {
                gameObject.transform.Rotate(Vector3.up, -90);
            }
        }

        CheckNavigableControls();
        CheckForFuelStation();
        CheckForTrailer();
    }

    private void Update()
    {
        Debug.DrawRay(fuelStationRay, Vector3.down);
        if (Input.GetMouseButtonDown(0))
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
        if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Control")))
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

    private IEnumerable PerformMove(Vector3 endPosition)
    {
        yield return null;
    }

    private bool TileIsNavigable(Vector3 position)
    {
        return Physics.Raycast(position, Vector3.down, 10f, LayerMask.GetMask("Navigation"));
    }

    private void Harvest(Crop crop)
    {
        // Harvest only if there's enough space
        if (levelData.CurrentPlayerCrop >= levelData.MaxPlayerCrop)
            return;
        levelData.CurrentPlayerCrop++;

        crop.Harvest();
        // Play harvest anim
        OnHarvest.Invoke(crop);
    }

}
