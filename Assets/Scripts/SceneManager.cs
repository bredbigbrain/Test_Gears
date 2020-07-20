using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SceneManager : MonoSingleton<SceneManager>
{
    public GameObject miniGameContainer;
    public float fadeSpeed = 1f;

    [Header("For wide screen dumb scale")]
    public SpriteRenderer background;

    Coroutine fadeMiniGameCoroutine;

    void Start()
    {
        foreach (var sr in miniGameContainer.GetComponentsInChildren<SpriteRenderer>())
            sr.color = new Color(1, 1, 1, 0);
        OnFadeEnd(true);

        ScaleBackgroundToFitScreen();
    }

    void ScaleBackgroundToFitScreen()
    {
        Vector3 GetBGLeftEdge()
        {
            Vector3 leftEdge = background.transform.position;
            leftEdge.x -= background.bounds.size.x / 2;
            return leftEdge;
        };

        var camera = Camera.main;
        while (camera.WorldToScreenPoint(GetBGLeftEdge()).x > 0)
            background.transform.localScale *= 1.05f;
    }

    public void OnBackgroundClick()
    {
        if(GameManager.Instance.IsAvailable)
            FadeMiniGame(miniGameContainer.activeSelf);
    }

    public void FadeMiniGame(bool fade, float delay = 0f)
    {
        if (fadeMiniGameCoroutine == null || fade != miniGameContainer.activeSelf)
        {
            if (fadeMiniGameCoroutine != null)
                StopCoroutine(fadeMiniGameCoroutine);
            if (!fade)
                miniGameContainer.SetActive(true);
            fadeMiniGameCoroutine = StartCoroutine(FadeMinigameCoroutine(fade, delay));
        }
    }

    IEnumerator FadeMinigameCoroutine(bool fade, float delay = 0f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        var spriteRenderes = miniGameContainer.GetComponentsInChildren<SpriteRenderer>();
        float targetAlpha = fade ? 0 : 1;
        float currentAlpha = spriteRenderes[0].color.a;
        while (currentAlpha != targetAlpha)
        {
            float alpha = Mathf.Clamp(currentAlpha + Time.deltaTime * fadeSpeed * (fade ? -1 : 1), 0, 1);
            foreach (var sr in spriteRenderes)
                sr.color = new Color(1, 1, 1, alpha);
            currentAlpha = alpha;
            yield return null;
        }
        
        fadeMiniGameCoroutine = null;
        OnFadeEnd(fade);
    }

    void OnFadeEnd(bool fade)
    {
        if (fade)
        {
            miniGameContainer.SetActive(false);
            GameManager.Instance.ResetGears();
        }
    }
}
