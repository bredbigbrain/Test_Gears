using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axle : MonoBehaviour
{
    public Gear defaultGear;
    public Gear.Type requiredGearType = Gear.Type.Undefined;

    [HideInInspector]
    public Gear currentGear;

    Coroutine moveGearCoroutine;

    private void Awake()
    {
        currentGear = defaultGear;
    }

    public bool IsRequiredGearOnPlace()
    {
        return currentGear.type == requiredGearType;
    }
    public void ResetGear(Gear gear)
    {
        ForceMoveCoroutineEnd();

        currentGear = gear;
        currentGear.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));
        currentGear.isAwailable = true;
    }

    public void SetCurrentGear(Gear gear)
    {
        ForceMoveCoroutineEnd();
        currentGear = gear;
        moveGearCoroutine = StartCoroutine(MoveCurrentGear());
    }

    void ForceMoveCoroutineEnd()
    {
        if (moveGearCoroutine != null)
        {
            currentGear.transform.position = transform.position;
            StopCoroutine(moveGearCoroutine);
            OnGearMoved(false);
        }
    }

    IEnumerator MoveCurrentGear()
    {
        currentGear.isAwailable = false;
        GameManager.Instance.animatedGearsCount++;
        while(currentGear.transform.position != transform.position)
        {
            currentGear.transform.position = Vector3.MoveTowards(currentGear.transform.position, transform.position, GameManager.Instance.gearMoveSpeed * Time.deltaTime);
            yield return null;
        }

        OnGearMoved(true);
    }

    void OnGearMoved(bool moveCompleted)
    {
        currentGear.isAwailable = true;
        GameManager.Instance.animatedGearsCount--;
        if (moveCompleted && IsRequiredGearOnPlace())
            GameManager.Instance.CheckWin();
        moveGearCoroutine = null;
    }
}
