using UnityEngine;

using UnityEngine.UI;
public class FloatingHealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    public Transform target;

    public void UpdateHealthBar(float currentValue, float maxValue) {

        slider.value = currentValue / maxValue;
    
    }

    public void Update()
    {
        transform.LookAt(target);
    }


}
