using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TankCreator : MonoBehaviour
{
    [Tooltip("1-Hull, 2-Turret, 3-Gun")]
    [SerializeField] private List<Button> SectionButtons;

    #region Part Prefabs

    private List<PartSave> _allParts = new();
    #endregion

    [SerializeField] private PartStatsWindow _partStatsWindow;
    [SerializeField] private TankBehaviour _firstTank;
    [SerializeField] private PartButton _partButtonPrefab;
    [SerializeField] private RectTransform _partButtonParent;

    [SerializeField] private PartSlot _partSlotPrefab;
    [SerializeField] private List<PartSlot> _partSlotsList;

    private Dictionary<ITankPart, PartSave> _createdParts = new();
    private List<PartButton> _createdButtons = new();
    private ITankPart _currentPart;
    private PartSave _currentPartSave;

    private ITankPart _currentHull;

    private int _currentSort = -1;

    private void Start()
    {
        PlayerInstance.Instance.OnDataLoaded += Initialize;

        SectionButtons[0].onClick.AddListener(() =>
        {
            ClearButtons();
            CreatePartButtons(_allParts);
            _currentSort = -1;
        });


        SectionButtons[1].onClick.AddListener(() =>
        {
            ClearButtons();
            CreatePartButtons(GetSortedParts(PartType.Hull));

        });
        SectionButtons[2].onClick.AddListener(() =>
        {
            ClearButtons();
            CreatePartButtons(GetSortedParts(PartType.Turret));

        });
        SectionButtons[3].onClick.AddListener(() =>
        {
            ClearButtons();
            CreatePartButtons(GetSortedParts(PartType.Gun));

        });

        _partStatsWindow.OnButtoPressed += SellOrDisconect;

    }

    [ContextMenu("Initialize")]
    private void Initialize()
    {
        PlayerInstance.Instance.OnDataPartsUpdated += UpdateData;

        if (PlayerInstance.Instance.PlayerData.CurrentTank == null)
        {
            GetFirstTank();
            PlayerInstance.Instance.Save();
        }

        ClearHull();
        _currentHull = PartSaveToTankParts(PlayerInstance.Instance.PlayerData.CurrentTank, out var createdParts);

        foreach (var item in createdParts)
            _createdParts.Add(item.Value, item.Key);

        _allParts = PlayerInstance.Instance.PlayerData.InventoryParts;

        UpdateData();

    }

    private void UpdateData()
    {
        ClearButtons();

        if (_currentSort == -1)
            CreatePartButtons(_allParts);
        else
            CreatePartButtons(GetSortedParts((PartType)_currentSort));

    }

    private void GetFirstTank()
    {
        Debug.Log("GetFirstTank");
        _firstTank.HullArmor.PartContainer.FindMyParts();
        _firstTank.HullArmor.PartContainer.TurretPlace[0].Part.PartContainer.FindMyParts();
        PlayerInstance.Instance.PlayerData.CurrentTank = TankPartsToPartSave(_firstTank.HullArmor);
    }

    private void SellOrDisconect()
    {
        if (_currentPart.IsBuiltIn)
            return;

        if (GetPartsList(_currentHull).Contains(_currentPart))
        {
            //Disconnect\\

            if (_createdParts[_currentPart].PartSlotsTurret.Count > 0 &&
            _createdParts[_currentPart].PartSlotsGuns.Count > 0)
            {
                Debug.Log("Can t remove while attached parts");
                return;
            }

            foreach (var item in _createdParts.Values)
            {
                item.RemovePartFromTurrets(_createdParts[_currentPart]);
                item.RemovePartFromGuns(_createdParts[_currentPart]);
            }

            foreach (var part in _createdParts.Keys)
            {
                if (part.PartContainer == null)
                    continue;

                foreach (var slot in part.PartContainer.TurretPlace)
                {
                    if (slot.Part == _currentPart)
                        slot.Part = null;
                }

                foreach (var slot in part.PartContainer.GunPlace)
                {
                    if (slot.Part == _currentPart)
                        slot.Part = null;
                }
            }


            _allParts.Add(_createdParts[_currentPart]);
            _createdParts.Remove(_currentPart);
            Destroy(_currentPart.GameObject);

            _currentPart = null;
            _currentPartSave = null;

            //_currentPart
            ShowSlots(true);

            UpdateData();
            _partStatsWindow.ClearDescription();
        }
        else
        {
            if (_currentPart == null)
                return;

            //Sell\\
            PlayerInstance.Instance.ChangeMoney(10);

            _allParts.Remove(_currentPartSave);

            Destroy(_currentPart.GameObject);


            _currentPart = null;
            _currentPartSave = null;

            UpdateData();
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var part = hit.collider.GetComponentInParent<ITankPart>();

                if (part != null)
                {
                    _currentPart = part;
                    _currentPartSave = _createdParts[_currentPart];

                    _partStatsWindow.ClearDescription();
                    _partStatsWindow.AddDescription(_currentPart.GetDescriptions().ToArray());

                    return;
                }

                PlacePart(hit.collider.GetComponent<PartSlot>());
            }
        }
    }

    private List<PartSave> GetSortedParts(PartType partType)
    {
        _currentSort = (int)partType;

        List<PartSave> parts = new();

        foreach (var item in _allParts)
        {
            if (item.PartType == partType)
                parts.Add(item);
        }

        return parts;
    }

    private void CreatePartButtons(List<PartSave> parts)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            int index = i;
            var partSave = parts[index];

            var pB = Instantiate(_partButtonPrefab);
            pB.transform.SetParent(_partButtonParent);
            _createdButtons.Add(pB);
            pB.Text.text = parts[index].PartName;
            pB.Button.onClick.AddListener(() => SetCurrentPart(partSave, CreatePart(PartsLoader.LoadPart(parts[index].PartName, parts[index].PartType).GetComponent<ITankPart>())));
        }
    }

    private void SetCurrentPart(PartSave partSave, ITankPart tankPart)
    {
        if (_currentPart != null && !GetPartsList(_currentHull).Contains(_currentPart))
        {
            _createdParts.Remove(_currentPart);
            Destroy(_currentPart.GameObject);
        }

        _currentPart = tankPart;
        _currentPartSave = partSave;
        _partStatsWindow.ClearDescription();
        _partStatsWindow.AddDescription(_currentPart.GetDescriptions().ToArray());

        _createdParts.Add(_currentPart, _currentPartSave);

        if (tankPart.PartContainer != null)
        {
            foreach (var slot in tankPart.PartContainer.TurretPlace)
                if (slot.Part != null)
                    _createdParts.Add(slot.Part, ConversionFromPart(slot.Part));

            foreach (var slot in tankPart.PartContainer.GunPlace)
                if (slot.Part != null)
                    _createdParts.Add(slot.Part, ConversionFromPart(slot.Part));
        }


        if (tankPart is HullArmor)
        {
            if (_currentHull != null)
                ClearHull();

            _allParts.Remove(_currentPartSave);
            _currentHull = _currentPart;
            _currentPartSave = null;
            _currentPart = null;


        }

        SaveParts();

        ShowSlots(true);
    }

    private static ITankPart CreatePart(ITankPart tankPartPrefab)
    {
        var part = Instantiate(tankPartPrefab.GameObject).GetComponent<ITankPart>();
        part.GameObject.name = tankPartPrefab.GameObject.name;

        if (part.PartContainer != null)
        {
            foreach (var slot in part.PartContainer.TurretPlace)
            {
                if (slot.Parent.childCount <= 0)
                    continue;

                var child = slot.Parent.GetChild(0);

                if (child != null)
                    slot.Part = child.gameObject.GetComponent<ITankPart>();
            }

            foreach (var slot in part.PartContainer.GunPlace)
            {
                if (slot.Parent.childCount <= 0)
                    continue;

                var child = slot.Parent.GetChild(0);

                if (child != null)
                    slot.Part = child.gameObject.GetComponent<ITankPart>();
            }
        }
        return part;
    }

    private void ClearButtons()
    {
        foreach (var item in _createdButtons)
            Destroy(item.gameObject);

        _createdButtons.Clear();
    }

    private void PlacePart(PartSlot partSlot)
    {
        //Debug.Log("PlacePart");

        if (partSlot == null)
            return;

        partSlot.Slot.Part = _currentPart;
        _currentPart.GameObject.transform.SetParent(partSlot.Slot.Parent);
        _currentPart.GameObject.transform.localPosition = Vector3.zero;

        _allParts.Remove(_currentPartSave);
        _currentPart = null;
        _currentPartSave = null;

        SaveParts();

        ShowSlots(true);

        UpdateData();
    }

    private void SaveParts()
    {
        PlayerInstance.Instance.PlayerData.CurrentTank = TankPartsToPartSave(_currentHull);
        PlayerInstance.Instance.Save();

    }

    private void ShowSlots(bool isTurret)
    {
        ClearSlots();

        Queue<ITankPart> partQueue = new();

        partQueue.Enqueue(_currentHull);

        ITankPart currentQueuePart;

        while (partQueue.Count != 0)
        {
            currentQueuePart = partQueue.Dequeue();

            if (currentQueuePart.PartContainer == null)
                continue;

            var containerSlots = currentQueuePart.PartContainer.TurretPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part == null)
                    CreateSlotForPart(containerSlots[i], containerSlots[i].Parent, i);
                else
                    partQueue.Enqueue(containerSlots[i].Part);
            }

            containerSlots = currentQueuePart.PartContainer.GunPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part == null)
                    CreateSlotForPart(containerSlots[i], containerSlots[i].Parent, i);
                else
                    partQueue.Enqueue(containerSlots[i].Part);
            }
        }
    }

    private static List<ITankPart> GetPartsList(ITankPart FirstPart)
    {
        List<ITankPart> allParts = new();

        Queue<ITankPart> partQueue = new();
        if (FirstPart == null)
            return allParts;


        partQueue.Enqueue(FirstPart);
        allParts.Add(FirstPart);

        ITankPart currentQueuePart;

        while (partQueue.Count != 0)
        {
            currentQueuePart = partQueue.Dequeue();

            if (currentQueuePart == null || currentQueuePart.PartContainer == null)
                continue;

            var containerSlots = currentQueuePart.PartContainer.TurretPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part != null)
                {
                    partQueue.Enqueue(containerSlots[i].Part);
                    allParts.Add(containerSlots[i].Part);
                }
            }

            containerSlots = currentQueuePart.PartContainer.GunPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part != null)
                {
                    partQueue.Enqueue(containerSlots[i].Part);
                    allParts.Add(containerSlots[i].Part);
                }
            }
        }

        return allParts;
    }

    private void CreateSlotForPart(PartContainerSlot container, Transform parent, int index)
    {
        var slot = Instantiate(_partSlotPrefab);
        slot.Slot = container;
        slot.transform.position = parent.position;
        slot.Index = index;

        _partSlotsList.Add(slot);
    }

    private void ClearSlots()
    {
        foreach (var item in _partSlotsList)
        {
            Destroy(item.gameObject);
        }

        _partSlotsList.Clear();
    }

    private void ClearHull()
    {
        foreach (var item in GetPartsList(_currentHull))
        {

            _allParts.Add(_createdParts[item]);
            _createdParts[item].RemoveAll();
            _createdParts.Remove(item);
            Destroy(item.GameObject);
        }

    }

    #region Save and Load

    public static PartSave TankPartsToPartSave(ITankPart hullTankPart)
    {
        Dictionary<ITankPart, PartSave> partsToSave = new();
        Queue<ITankPart> partQueue = new();

        partQueue.Enqueue(hullTankPart);
        ITankPart currentQueuePart;
        PartSave currentPartSave;

        partsToSave.Add(hullTankPart, ConversionFromPart(hullTankPart));

        while (partQueue.Count != 0)
        {
            currentQueuePart = partQueue.Dequeue();
            currentPartSave = partsToSave[currentQueuePart];

            if (currentQueuePart.PartContainer == null)
                continue;

            var containerSlots = currentQueuePart.PartContainer.TurretPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part != null)
                {
                    partQueue.Enqueue(containerSlots[i].Part);

                    var newPart = ConversionFromPart(containerSlots[i].Part);
                    partsToSave.Add(containerSlots[i].Part, newPart);
                    currentPartSave.AddToList(currentPartSave.PartSlotsTurret, i, newPart);
                }
            }

            containerSlots = currentQueuePart.PartContainer.GunPlace;
            for (int i = 0; i < containerSlots.Count; i++)
            {
                if (containerSlots[i].Part != null)
                {
                    partQueue.Enqueue(containerSlots[i].Part);

                    var newPart = ConversionFromPart(containerSlots[i].Part);
                    partsToSave.Add(containerSlots[i].Part, newPart);
                    currentPartSave.AddToList(currentPartSave.PartSlotsGuns, i, newPart);

                }
            }
        }

        return partsToSave[hullTankPart];
    }

    public static ITankPart PartSaveToTankParts(PartSave loadingPart)
    {

        ITankPart firstPart;

        Dictionary<PartSave, ITankPart> convertedParts = new();
        Queue<PartSave> partQueue = new();

        firstPart = CreatePart(PartsLoader.LoadPart(loadingPart.PartName, loadingPart.PartType).GetComponent<HullArmor>());

        convertedParts.Add(loadingPart, firstPart);

        partQueue.Enqueue(loadingPart);
        ITankPart currentQueuePart;
        PartSave currentPartSave;

        while (partQueue.Count != 0)
        {
            currentPartSave = partQueue.Dequeue();
            currentQueuePart = convertedParts[currentPartSave];

            if (currentQueuePart.PartContainer == null)
                continue;

            var containerSlots = currentPartSave.PartSlotsTurret;

            foreach (var item in containerSlots)
            {
                partQueue.Enqueue(item.partSave);
                var tankPart = CreatePart(PartsLoader.LoadPart(item.partSave.PartName, item.partSave.PartType).GetComponent<ITankPart>());

                currentQueuePart.PartContainer.TurretPlace[item.partIndex].Part = tankPart;
                tankPart.GameObject.transform.SetParent(currentQueuePart.PartContainer.TurretPlace[item.partIndex].Parent);
                tankPart.GameObject.transform.localPosition = Vector3.zero;

                convertedParts.Add(item.partSave, tankPart);
            }

            containerSlots = currentPartSave.PartSlotsGuns;

            foreach (var item in containerSlots)
            {
                partQueue.Enqueue(item.partSave);
                var tankPart = CreatePart(PartsLoader.LoadPart(item.partSave.PartName, item.partSave.PartType).GetComponent<ITankPart>());

                currentQueuePart.PartContainer.GunPlace[item.partIndex].Part = tankPart;
                tankPart.GameObject.transform.SetParent(currentQueuePart.PartContainer.GunPlace[item.partIndex].Parent);
                tankPart.GameObject.transform.localPosition = Vector3.zero;

                convertedParts.Add(item.partSave, tankPart);
            }
        }
        return firstPart;
    }

    public static ITankPart PartSaveToTankParts(PartSave loadingPart, out Dictionary<PartSave, ITankPart> createdParts)
    {

        ITankPart firstPart;

        Dictionary<PartSave, ITankPart> convertedParts = new();
        Queue<PartSave> partQueue = new();

        firstPart = CreatePart(PartsLoader.LoadPart(loadingPart.PartName, loadingPart.PartType).GetComponent<HullArmor>());

        convertedParts.Add(loadingPart, firstPart);

        partQueue.Enqueue(loadingPart);
        ITankPart currentQueuePart;
        PartSave currentPartSave;

        while (partQueue.Count != 0)
        {
            currentPartSave = partQueue.Dequeue();
            currentQueuePart = convertedParts[currentPartSave];

            if (currentQueuePart.PartContainer == null)
                continue;

            var containerSlots = currentPartSave.PartSlotsTurret;

            foreach (var item in containerSlots)
            {
                partQueue.Enqueue(item.partSave);
                var tankPart = CreatePart(PartsLoader.LoadPart(item.partSave.PartName, item.partSave.PartType).GetComponent<ITankPart>());

                currentQueuePart.PartContainer.TurretPlace[item.partIndex].Part = tankPart;
                tankPart.GameObject.transform.SetParent(currentQueuePart.PartContainer.TurretPlace[item.partIndex].Parent);
                tankPart.GameObject.transform.localPosition = Vector3.zero;

                convertedParts.Add(item.partSave, tankPart);
            }

            containerSlots = currentPartSave.PartSlotsGuns;

            foreach (var item in containerSlots)
            {
                partQueue.Enqueue(item.partSave);
                var tankPart = CreatePart(PartsLoader.LoadPart(item.partSave.PartName, item.partSave.PartType).GetComponent<ITankPart>());

                currentQueuePart.PartContainer.GunPlace[item.partIndex].Part = tankPart;
                tankPart.GameObject.transform.SetParent(currentQueuePart.PartContainer.GunPlace[item.partIndex].Parent);
                tankPart.GameObject.transform.localPosition = Vector3.zero;

                convertedParts.Add(item.partSave, tankPart);
            }
        }

        createdParts = convertedParts;

        return firstPart;
    }
    #endregion

    public static PartSave ConversionFromPart(ITankPart part)
    {
        if (part == null)
            return null;

        PartSave partSave = new();

        partSave.PartType = PartTypeSwitcher.GetEnumByType(part.GetType());
        partSave.PartName = part.GameObject.name;

        return partSave;
    }
}
