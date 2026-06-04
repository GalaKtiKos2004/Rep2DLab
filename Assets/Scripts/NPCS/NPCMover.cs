using System;
using UnityEngine;

public class NPCMover : MonoBehaviour
{
    [SerializeField] private StartDialogue _startDialogue;
    [SerializeField] private float _speed = 2f;
    
    public event Action TriggerEntered;

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger entered");
        TriggerEntered?.Invoke();
        _startDialogue.StartText();
        _speed = 0f;
    }
}
