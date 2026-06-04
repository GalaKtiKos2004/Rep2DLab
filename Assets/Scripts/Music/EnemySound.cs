using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private EnemyFighter _fighter;
    [SerializeField] private AudioClip _swordSound;

    private void OnEnable()
    {
        _fighter.Attacked += OnAttack;
    }

    private void OnDisable()
    {
        _fighter.Attacked -= OnAttack;
    }

    private void OnAttack()
    {
        _audioSource.PlayOneShot(_swordSound);
    }
}
