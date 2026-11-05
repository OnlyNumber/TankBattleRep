using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private PartButton _partButtonPrefab;
    [SerializeField] private List<RectTransform> _goodsPlaces;
    [SerializeField] private PartStatsWindow _partStatsWindow;


    #region Prefabs
    private HullArmor[] _hullArmorList;
    private TurretMovement[] _turretsList;
    private Gun[] _gunsList;
    #endregion

    private List<PartGoodsInfo> _partGoodsInfos = new();
    private PartGoodsInfo _currentGoods;

    private void Start()
    {
        _hullArmorList = PartsLoader.LoadParts<HullArmor>();
        _gunsList = PartsLoader.LoadParts<Gun>();
        _turretsList = PartsLoader.LoadParts<TurretMovement>();

        _partStatsWindow.OnButtoPressed += BuyCurrentItem;
        _partStatsWindow.ClearAll();
        _partStatsWindow.ActivateButton(false);

        CreateGoods();
    }

    private HashSet<ITankPart> GetAllParts()
    {
        HashSet<ITankPart> allPartsPool = new();

        foreach (var item in _hullArmorList)
            allPartsPool.Add(item);


        foreach (var item in _turretsList)
            allPartsPool.Add(item);

        foreach (var item in _gunsList)
            allPartsPool.Add(item);

        return allPartsPool;
    }

    private void CreateGoods()
    {
        var _allPartsPool = GetAllParts();

        foreach (var place in _goodsPlaces)
        {
            var tankPartPrefab = _allPartsPool.ElementAt(Random.Range(0, _allPartsPool.Count)).GameObject.GetComponent<ITankPart>();

            PartGoodsInfo currentPartInfo = new()
            {
                Price = 20,
                TankPart = tankPartPrefab,
                PartButton = Instantiate(_partButtonPrefab, place)
            };

            currentPartInfo.PartButton.RectTransform.localPosition = Vector2.zero;
            currentPartInfo.PartButton.Button.onClick.AddListener(() => ShowItemInfo(currentPartInfo));
            currentPartInfo.PartButton.PartNameText.text = AddSpacesToString(currentPartInfo.TankPart.GameObject.name);
            currentPartInfo.PartButton.PriceText.text = currentPartInfo.Price.ToString();

            _partGoodsInfos.Add(currentPartInfo);
        }
    }

    public static string AddSpacesToString(string name)
    {
        string spacedString = "";

        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]))
                spacedString += " ";

            spacedString += name[i];
        }

        return spacedString;
    }

    private void ShowItemInfo(PartGoodsInfo partGoodsInfo)
    {
        _currentGoods = partGoodsInfo;

        _partStatsWindow.ClearAll();
        _partStatsWindow.AddDescription(new Description("Price ", _currentGoods.Price.ToString()));
        _partStatsWindow.AddDescription(_currentGoods.TankPart.GetDescriptions().ToArray());
        _partStatsWindow.ActivateButton(_currentGoods.Price < PlayerInstance.Instance.PlayerData.MoneyCount);
        _partStatsWindow.SetPartName(AddSpacesToString(_currentGoods.TankPart.GameObject.name));

    }

    private void BuyCurrentItem()
    {
        PlayerInstance.Instance.ChangeMoney(-_currentGoods.Price);
        PlayerInstance.Instance.PlayerData.InventoryParts.Add(TankCreator.ConversionFromPart(_currentGoods.TankPart));

        _partGoodsInfos.Remove(_currentGoods);
        Destroy(_currentGoods.PartButton.gameObject);
        _partGoodsInfos.Remove(_currentGoods);
        _currentGoods = null;

        _partStatsWindow.ClearAll();
        _partStatsWindow.ActivateButton(false);

        PlayerInstance.Instance.Save();

        PlayerInstance.Instance.UpdateData();


    }
}

public class PartGoodsInfo
{
    public int Price;
    public ITankPart TankPart;
    public PartButton PartButton;

}