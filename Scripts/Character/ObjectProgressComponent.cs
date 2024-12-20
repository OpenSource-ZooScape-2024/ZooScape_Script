using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProgressComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private float StartZPos;
    private float EndZPos;

    void Start()
    {
        if (GameMode_.instance != null)
        {
            StartZPos = GameMode_.instance.startPoint.position.z;
            EndZPos = GameMode_.instance.endPoint.position.z;
        }
    }

    public float _progressPoint = 0f;
    // Update is called once per frame
    void Update()
    {
        _progressPoint = (transform.position.z - StartZPos) / (EndZPos - StartZPos);
    }
}
