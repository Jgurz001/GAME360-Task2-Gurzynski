using UnityEngine;

using UnityEngine.UI;
public class FloatingHealthBar : MonoBehaviour
{
    // Slider that will display the healthbar from inspector
    [SerializeField] private Slider slider;
    //Obkect the health bar faces
    public Transform target;

    //Updates the displayed health percentage
    public void UpdateHealthBar(float currentValue, float maxValue) {

        //Convert current health to percentage for slider
        slider.value = currentValue / maxValue;
    
    }

    private void Update()
    {
        // Keeps health bar facing the target
        transform.LookAt(target);
    }


}
