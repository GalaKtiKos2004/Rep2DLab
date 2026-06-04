using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackBox : MonoBehaviour
{
    [SerializeField] private FinishDialog _finishDialog;
    [SerializeField] private float _fadeDuration = 3f;
    
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _finishDialog.DialogueFinished += FadeToBlack;
    }

    private void OnDisable()
    {
        _finishDialog.DialogueFinished -= FadeToBlack;
    }

    private void FadeToBlack()
    {
        _image.DOFade(1f, _fadeDuration);
    }
}
