using UnityEngine;

/// <summary>
/// NPC, который выдаёт квест при разговоре (по клавише E рядом с триггером).
/// </summary>
public class QuestNpc : MonoBehaviour
{
    private const KeyCode TalkKey = KeyCode.E;

    [SerializeField] private Trigger _trigger;
    [SerializeField] private StartDialogue _dialogue;
    [SerializeField] private Quest _quest;

    private bool _playerNearby;

    private void OnEnable()
    {
        _trigger.TriggerEntered += OnPlayerEntered;
        _trigger.TriggerExited += OnPlayerExited;
    }

    private void OnDisable()
    {
        _trigger.TriggerEntered -= OnPlayerEntered;
        _trigger.TriggerExited -= OnPlayerExited;
    }

    private void Update()
    {
        if (_playerNearby == false || Input.GetKeyDown(TalkKey) == false)
        {
            return;
        }

        Talk();
    }

    private void Talk()
    {
        if (_quest == null || QuestManager.Instance == null)
        {
            return;
        }

        QuestProgressData progress = QuestManager.Instance.GetProgress(_quest.Id);

        if (progress.State == QuestState.Completed)
        {
            _dialogue.SetDialogues(new[] { "Спасибо за помощь. Ты уже сделал достаточно." });
            _dialogue.StartText();
            return;
        }

        if (progress.State == QuestState.Active)
        {
            _dialogue.SetDialogues(new[]
            {
                $"Квест «{_quest.Title}» ещё в процессе.",
                _quest.Description
            });
            _dialogue.StartText();
            return;
        }

        QuestManager.Instance.TryAcceptQuestWithDialogue(_quest);
    }

    private void OnPlayerEntered(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
        {
            _playerNearby = true;
        }
    }

    private void OnPlayerExited(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
        {
            _playerNearby = false;
        }
    }
}
