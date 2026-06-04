using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class StartDialogue : MonoBehaviour
{
    [SerializeField] private List<string> _dialogues;
    [SerializeField] private float _textDuration;
    [SerializeField] private float _wait;
    
    private Text _text;
    private WaitForSeconds _waitForSeconds;
    private int _dialogueCount = 0;
    
    public event Action DialogueFinished;

    private void Awake()
    {
        _text = GetComponent<Text>();
        _waitForSeconds = new WaitForSeconds(_wait);
    }

    public void StartText()
    {
        PrintText();
    }

    private void PrintText()
    {
        if (_dialogueCount == _dialogues.Count)
        {
            DialogueFinished?.Invoke();
            return;
        }
        
        _text.text = "";
        _text.DOText(_dialogues[_dialogueCount++], _textDuration).SetEase(Ease.Linear);
        StartCoroutine(DialogueDelay());
    }

    private IEnumerator DialogueDelay()
    {
        yield return _waitForSeconds;
        PrintText();
    }
}
