using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundChange : MonoBehaviour
{
    public string playerPrefsKey = "SoundVolume";
    public Scrollbar scrollbar;

    void Start()
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            float savedValue = PlayerPrefs.GetFloat(playerPrefsKey);
            scrollbar.value = savedValue;
            // Применяем сохраненное значение громкости к нужному объекту
            // Например, можно изменить громкость аудио
            // exampleObject.GetComponent<AudioSource>().volume = savedValue;
        }

        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    public void OnScrollbarValueChanged(float value)
    {
        PlayerPrefs.SetFloat(playerPrefsKey, value);
        PlayerPrefs.Save();
        // Применяем значение громкости к нужному объекту при изменении скроллбара
        // exampleObject.GetComponent<AudioSource>().volume = value;
    }
}
