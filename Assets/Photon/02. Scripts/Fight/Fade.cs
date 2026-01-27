using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public GameObject endingUI;
    public Image fadeImage;
    public static Action<float, Color, bool> onFadeAction;
    
    void OnEnable()
    {
        onFadeAction += OnFade;
    }

    void OnDisable()
    {
        onFadeAction -= OnFade;
    }

    private void OnFade(float t, Color c, bool isFade)
    {
        StartCoroutine(FadeRoutine(t, c, isFade));
    }

    IEnumerator FadeRoutine(float fadeTime, Color color, bool isFade)
    {
        fadeImage.gameObject.SetActive(true);

        float timer = 0f;
        float percent = 0f;
        while (percent < 1f)
        {
            timer += Time.deltaTime;
            percent = timer / fadeTime;

            float fadeValue = isFade ? percent : 1 - percent;

            fadeImage.color = new Color(color.r, color.g, color.b, fadeValue);

            yield return null;
        }
        
        endingUI.SetActive(true);
    }
}