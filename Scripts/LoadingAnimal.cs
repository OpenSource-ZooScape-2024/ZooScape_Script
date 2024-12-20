using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetBool("isMoving", true);
    }
}
