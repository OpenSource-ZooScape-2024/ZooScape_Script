using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelUI : MonoBehaviour
{
    public static LoadingPanelUI instance;
    public float rotationSpeed = -100f;
    public Image image;
    public bool _isLoadingPage = false;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(!_isLoadingPage)
            gameObject.SetActive(false);
    }

    void Update()
    {
        image.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
