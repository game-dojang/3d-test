using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image gauge;

    public void SetHPGauge(float hp)
    {
        gauge.fillAmount = hp;
    }
}
