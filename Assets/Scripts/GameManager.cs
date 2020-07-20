using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public float gearMoveSpeed = 1f;
    public float gearRotationSpeed = 60f;
    public float winAnimationTime = 2f;

    public Axle[] axles;
    public CircleCollider2D leftStaticGear, rightStaticGear;

    [HideInInspector]
    public int animatedGearsCount = 0;

    Coroutine winAnimationCoroutine;   

    public bool IsAvailable
    {
        get => animatedGearsCount == 0 && winAnimationCoroutine == null;
    }

    public void ResetGears()
    {
        foreach (var axle in axles)
            axle.ResetGear(axle.defaultGear);
    }

    public Axle GetCurrentAxle(Gear gear)
    {
        foreach (var axle in axles)
        {
            if (axle.currentGear == gear)
                return axle;
        }
        Debug.Assert(false);
        return null;
    }

    public void ResetGear(Gear gear)
    {
        GetCurrentAxle(gear)?.ResetGear(gear);
    }

    public void SwapGears(Gear gear1, Gear gear2)
    {
        var axle1 = GetCurrentAxle(gear1);
        var axle2 = GetCurrentAxle(gear2);

        Debug.Assert(axle1 != null && axle2 != null);
        if (axle1 == null || axle2 == null)
            return;

        axle1.SetCurrentGear(gear2);
        axle2.SetCurrentGear(gear1);
    }
    public void CheckWin()
    {
        if (animatedGearsCount != 0)
            return;
        foreach (var axle in axles)
        {
            if (!axle.IsRequiredGearOnPlace())
                return;
        }
        OnWin();
    }

    void OnWin()
    {
        if(winAnimationCoroutine == null)
        {
            SoundsManager.Instance.Play(SoundsManager.ClipType.Win);
            foreach (var axle in axles)
                axle.currentGear.isAwailable = false;
            winAnimationCoroutine = StartCoroutine(AnimateGears());
        }
    }

    IEnumerator AnimateGears()
    {
        int direction = 1;
        void Rotate(Transform transform) 
        {
            float angle = gearRotationSpeed * Mathf.PI * Time.deltaTime / transform.GetComponent<CircleCollider2D>().radius;
            transform.Rotate(Vector3.forward, angle * direction, Space.Self);
            direction *= -1;
        };

        SceneManager.Instance.FadeMiniGame(true, winAnimationTime);

        float fadeAinimationTime = 1f / SceneManager.Instance.fadeSpeed;
        float time = winAnimationTime + fadeAinimationTime;
        while (time > 0)
        {
            direction = 1;
            Rotate(leftStaticGear.transform);
            foreach (var axle in axles)
                Rotate(axle.currentGear.transform);
            Rotate(rightStaticGear.transform);
            time -= Time.deltaTime;
            yield return null;
        }

        winAnimationCoroutine = null;
    }

    void OnDisable()
    {
        if(winAnimationCoroutine != null)
        {
            StopCoroutine(winAnimationCoroutine);
            winAnimationCoroutine = null;
        }
    }
}
