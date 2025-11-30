using System;
using UnityEngine;
using Minge2025Summer.Scripts.InGame.PlayerTransformScript;

public class Puddle : MonoBehaviour
{
    [SerializeField] private PlayerPositionModel model;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) model.IsPuddling = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) model.IsPuddling = false;
    }
}
