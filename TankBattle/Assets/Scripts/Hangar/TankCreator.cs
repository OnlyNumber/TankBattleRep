using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject.SpaceFighter;

public class TankCreator : MonoBehaviour
{


    [Tooltip("1-Hull, 2-Turret, 3-Gun")]
    public List<Button> SectionButtons;

    #region Part Prefabs

    private List<ITankPart> _allParts = new();
    #endregion

    [SerializeField] private TankBehaviour _firstTank;
    [SerializeField] private PartButton _partButtonPrefab;
    [SerializeField] private RectTransform _partButtonParent;

    [SerializeField] private PartSlot _partSlotPrefab;
    [SerializeField] private List<PartSlot> _partSlotsList;

    private List<PartButton> _createdButtons = new();
    private ITankPart _currentPart;
    private ITankPart _currentHull;

    private int _currentSort;

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

    }

    private void Initialize()
    {
        PlayerInstance.Instance.OnDataPartsUpdated += UpdateData;

        PlayerInstance.Instance.PlayerData.CurrentTank = null;

        if (PlayerInstance.Instance.PlayerData.CurrentTank == null)
        {
            GetFirstTank();
            PlayerInstance.Instance.Save();
        }

        ClearHull();
        _currentHull = PartSaveToTankParts(PlayerInstance.Instance.PlayerData.CurrentTank);

        UpdateData();

    }

    private void UpdateData()
    {
        ClearButtons();
        _allParts.Clear();

        foreach (var item in PlayerInstance.Instance.PlayerData.InventoryParts)
        {
            _allParts.Add(PartsLoader.LoadPart(item.PartName, item.PartType).GetComponent<ITankPart>());

        }

        if (_currentSort == -1)
            CreatePartButtons(_allParts);
        else
            CreatePartButtons(GetSortedParts((PartType)_currentSort));

    }


    private void GetFirstTank()
    {
        _firstTank.HullArmor.PartContainer.FindMyParts();
        _firstTank.HullArmor.PartContainer.TurretPlace[0].Part.PartContainer.FindMyParts();
        PlayerInstance.Instance.PlayerData.CurrentTank = TankPartsToPartSave(_firstTank.HullArmor);
    }


    private void Update()
    {
        if (_currentPart != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
                PlacePart(hit.collider.GetComponent<PartSlot>());
        }
    }

    private List<ITankPart> GetSortedParts(PartType partType)
    {
        _currentSort = (int)partType;

        List<ITankPart> parts = new();
        Type type = PartTypeSwitcher.GetTypeByEnum(partType);

        foreach (var item in _allParts)
        {
            if (item.GetType() == type)
                parts.Add(item);
        }

        return parts;
    }

    private void CreatePartButtons(List<ITankPart> parts)
    {
        //ClearButtons();

        for (int i = 0; i < parts.Count; i++)
        {
            int index = i;

            var pB = Instantiate(_partButtonPrefab);
            pB.transform.SetParent(_partButtonParent);
            _createdButtons.Add(pB);
            pB.Text.text = parts[index].GameObject.name;
            pB.Button.onClick.AddListener(() => SetCurrentPart(CreatePart(parts[index])));
        }
    }

    private void SetCurrentPart(ITankPart tankPart)
    {
        if (_currentPart != null)
            Destroy(_currentPart.GameObject);

        _currentPart = tankPart;

        if (tankPart is HullArmor)
        {
            if (_currentHull != null)
                ClearHull();

            _currentHull = _currentPart;
            _currentPart = null;
        }

        ShowSlots(true);
    }

    private static ITankPart CreatePart(ITankPart tankPartPrefab)
    {
        var part = Instantiate(tankPartPrefab.GameObject).GetComponent<ITankPart>();
        part.GameObject.name = tankPartPrefab.GameObject.name;
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
        if (partSlot == null)
            return;

        partSlot.Slot.Part = _currentPart;
        _currentPart.GameObject.transform.SetParent(partSlot.Slot.Parent);
        _currentPart.GameObject.transform.localPosition = Vector3.zero;

        //_createdParts.Add(_currentPart);
        _currentPart = null;
        ShowSlots(true);

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
            Destroy(item.GameObject);
        }

    }

    #region Save and Load

    public static PartSave TankPartsToPartSave(ITankPart hullTankPart)
    {
        Dictionary<ITankPart, PartSave> partsToSave = new();
        Queue<ITankPart> partQueue = new();

        Debug.Log(hullTankPart.GameObject.transform.parent.name);

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
