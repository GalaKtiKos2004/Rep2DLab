using UnityEngine;

public class PlayerSlower : MonoBehaviour
{
    [SerializeField] private PlayerMover _playerMover;
    
    public PlayerMover PlayerMover => _playerMover;
}
