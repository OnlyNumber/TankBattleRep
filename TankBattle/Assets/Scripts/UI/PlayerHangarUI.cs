using UnityEngine;
using UnityEngine.UI;

public class PlayerHangarUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _moneyText;
    [SerializeField] private Button BattleButton;


    void Start()
    {
        PlayerInstance.Instance.OnDataLoaded += Initialize;
    }

    public void Initialize()
    {
        PlayerInstance.Instance.OnChangeMoney += MomeyTextUpdate;
        _moneyText.text = PlayerInstance.Instance.PlayerData.MoneyCount.ToString();
        BattleButton.onClick.AddListener(LoadBattle);
        PlayerInstance.Instance.OnDataLoaded -= Initialize;

    }


    private void MomeyTextUpdate(int moneyCount)
    {
        _moneyText.text = moneyCount.ToString();
    }

    private void LoadBattle()
    {
        SceneController.LoadScene(SceneIndex.GamePlay);
    }
}
