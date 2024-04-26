using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected Gradient gradient;
    [SerializeField] protected Image fill;

    public void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetValue(float value)
    {
        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
