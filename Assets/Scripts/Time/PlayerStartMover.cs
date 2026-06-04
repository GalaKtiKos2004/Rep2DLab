using UnityEngine;

public class PlayerStartMover : MonoBehaviour
{
    [SerializeField] private StartDialogue _finishDialog;
    [SerializeField] private float _speed = 1f;
    
    private bool _canMove = false;

    private void OnEnable()
    {
        _finishDialog.DialogueFinished += Move;
    }

    private void OnDisable()
    {
        _finishDialog.DialogueFinished -= Move;
    }

    private void Update()
    {
        if (_canMove == false)
        {
            return;
        }
        
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }
    
    private void Move() => _canMove = true;
}
