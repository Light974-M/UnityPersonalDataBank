using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextDisplayer : MonoBehaviour
{
    [SerializeField, Tooltip("value of slider")]
    private Text _value;

    private Slider _slider;

    private void Awake()
    {
        if(_slider == null)
            if(!TryGetComponent(out _slider))
                _slider = gameObject.AddComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _value.text = ((int)Mathf.Round(_slider.value)).ToString();
    }
}
