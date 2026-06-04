using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackBoxStart : MonoBehaviour
{
    [SerializeField] private StartDialogue _startDialogue;
    [SerializeField] private float _fadeDuration = 3f;
    [SerializeField] private FirstSceneLoader _firstSceneLoader;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _startDialogue.DialogueFinished += FadeToBlack;
    }

    private void OnDisable()
    {
        _startDialogue.DialogueFinished -= FadeToBlack;
    }

    private void FadeToBlack()
    {
        _image.DOFade(1f, _fadeDuration);
        StartCoroutine(NewScene());
    }

    private IEnumerator NewScene()
    {
        yield return new WaitForSeconds(_fadeDuration);
        
        _firstSceneLoader.SceneLoad();
    }

}
