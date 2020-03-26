using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyMonoBehaviour : MonoBehaviour
{
    public UnityAction UpdateAction;
    
    private void Awake()
    {
        Debug.Log("MyBehaviour----->Awake");
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Debug.Log("MyBehaviour----->Start");
    }

    private void Update()
    {
        Debug.Log("MyBehaviour----->Update");
        if (UpdateAction != null)
            UpdateAction();
    }

    private void OnDestroy()
    {
        Debug.Log("MyBehaviour----->OnDestroy");
    }
}
