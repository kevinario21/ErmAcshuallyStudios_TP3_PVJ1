using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    public Coroutine changeAlpha { get; private set; }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()  // negro a transparente
    {
        FadeEffect(0);
    }

    public void FadeOut() //transparente a negro
    {
        FadeEffect(1);
    }


    private void FadeEffect(float targetAlpha)
    {
        if (changeAlpha != null)
        {
            StopCoroutine(changeAlpha);
        }
        changeAlpha = StartCoroutine(ChangeAlpha(targetAlpha));
    }

    private IEnumerator ChangeAlpha(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
