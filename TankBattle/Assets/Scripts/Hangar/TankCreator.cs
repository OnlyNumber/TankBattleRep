using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TankCreator : MonoBehaviour
{
    [Tooltip("1-Hull, 2-Turret, 3-Gun")]
    public List<Button> SectionButtons;

    #region Prefabs
    private HullArmor[] _hullArmorList;
    private TurretMovement[] _turretsList;
    private Gun[] _gunsList;
    #endregion

    [SerializeField] private PartButton _partButtonPrefab;
    [SerializeField] private RectTransform _partButtonParent;

    [SerializeField] private PartSlot _partSlotPrefab;
    [SerializeField] private List<PartSlot> _partSlotsList;



    private List<PartButton> _createdButtons = new();
    private ITankPart _currentPart;
    private ITankPart _currentHull;

    private void Start()
    {
        _hullArmorList = PartsLoader.LoadParts<HullArmor>();
        _gunsList = PartsLoader.LoadParts<Gun>();
        _turretsList = PartsLoader.LoadParts<TurretMovement>();

        SectionButtons[0].onClick.AddListener(() => CreatePartButtons(_hullArmorList));
        SectionButtons[1].onClick.AddListener(() => CreatePartButtons(_turretsList));
        SectionButtons[2].onClick.AddListener(() => CreatePartButtons(_gunsList));
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

    private void CreatePartButtons(ITankPart[] parts)
    {
        ClearButtons();

        for (int i = 0; i < parts.Length; i++)
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

    private ITankPart CreatePart(ITankPart tankPartPrefab)
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

    private List<ITankPart> GetPartsList()
    {
        List<ITankPart> allParts = new();

        Queue<ITankPart> partQueue = new();
        if (_currentHull == null)
            return allParts;


        partQueue.Enqueue(_currentHull);
        allParts.Add(_currentHull);

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
        foreach (var item in GetPartsList())
        {
            Destroy(item.GameObject);
        }

        _hullArmorList = null;
    }

    #region Save and Load
    private PartSave testSave;


    [ContextMenu("TestSave")]
    private void TestSave()
    {
        var path = Path.Combine(Application.persistentDataPath, "Saves", "PlayerSave.json");

        JsonDataLoader.SaveAsync(path, SaveBuild()).Forget();
    }

    [ContextMenu("TestLoadBuild")]
    public void TestLoadBuild()
    {
        LoadGameAsync().Forget();
    }
    public PartSave SaveBuild()
    {
        Dictionary<ITankPart, PartSave> partsToSave = new();
        Queue<ITankPart> partQueue = new();

        partQueue.Enqueue(_currentHull);
        ITankPart currentQueuePart;
        PartSave currentPartSave;

        partsToSave.Add(_currentHull, ConversionFromPart(_currentHull));

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

        return partsToSave[_currentHull];
    }

    public void LoadBuild(PartSave loadingPart)
    {
        ClearHull();


        Dictionary<PartSave, ITankPart> convertedParts = new();
        Queue<PartSave> partQueue = new();

        if (loadingPart == null)
            Debug.Log("loadingPart == null");
        Debug.Log(loadingPart.PartName);

        _currentHull = CreatePart(PartsLoader.LoadPart(loadingPart.PartName, loadingPart.PartType).GetComponent<HullArmor>());

        convertedParts.Add(loadingPart, _currentHull);

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

    }



    public async UniTask<PartSave> LoadGameAsync()
    {
        var path = Path.Combine(Application.persistentDataPath, "Saves", "PlayerSave.json");

        testSave = await JsonDataLoader.LoadAsync<PartSave>(path);

        LoadBuild(testSave);
        return testSave;
    }
    #endregion

    private PartSave ConversionFromPart(ITankPart part)
    {
        if (part == null)
            return null;

        PartSave partSave = new();

        partSave.PartType = PartTypeSwitcher.GetEnumByType(part.GetType());
        partSave.PartName = part.GameObject.name;

        return partSave;
    }
}
