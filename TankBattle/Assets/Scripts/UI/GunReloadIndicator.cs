using UnityEngine;
using UnityEngine.UI;

public class GunReloadIndicator : MonoBehaviour
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Slider _circleSlider;

    public void Initialize(Gun gun)
    {
        _gun = gun;
    }

    void Update()
    {
        _circleSlider.value = _gun.GetCurretTime();

        _circleSlider.gameObject.SetActive(_gun.GetCurretTime() < 1);
    }
}
