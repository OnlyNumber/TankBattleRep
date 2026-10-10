using UnityEngine;

public class PlayerHangarUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _moneyText;

    void Start()
    {
        PlayerInstance.Instance.OnDataLoaded += Initialize;
    }

    public void Initialize()
    {
        PlayerInstance.Instance.OnChangeMoney += MomeyTextUpdate;
        _moneyText.text = PlayerInstance.Instance.PlayerData.MoneyCount.ToString();
        PlayerInstance.Instance.OnDataLoaded -= Initialize;

    }


    private void MomeyTextUpdate(int moneyCount)
    {
        _moneyText.text = moneyCount.ToString();
    }
}
