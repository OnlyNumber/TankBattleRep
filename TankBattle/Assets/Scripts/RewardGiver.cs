using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class RewardGiver
{
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private Vector3 _messageOffset = Vector3.up;
    [SerializeField] private int _reward;


    public GameObject RewardMessage(Vector3 messagePosition, int reward)
    {
        var message = GameObject.Instantiate(_messagePrefab);
        message.GetComponentInChildren<TMPro.TMP_Text>().text = reward.ToString();
        message.transform.position = messagePosition;

        PlayerInstance.Instance.ChangeMoney(reward);
        
        return message;
    }

    public void GetRewardForEnemy(TankBehaviour tankBehaviour)
    {
        var message = RewardMessage(tankBehaviour.transform.position + _messageOffset, _reward);
        WaitAndDelete(message).Forget();
    }

    private async UniTask WaitAndDelete(GameObject gameObject)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3));

        if (gameObject != null)
            GameObject.Destroy(gameObject);
    }
}
