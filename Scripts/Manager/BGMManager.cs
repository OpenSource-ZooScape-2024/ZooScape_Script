using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().volume = SettingManager.instance._musicVolume;
    }

    public void UpdateVolume()
    {
        GetComponent<AudioSource>().volume = SettingManager.instance._musicVolume;
    }
}
