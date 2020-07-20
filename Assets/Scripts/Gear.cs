using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class Gear : MonoBehaviour
{
    public Type type = Type.Undefined;

    [HideInInspector]
    public bool isAwailable = true;

    Vector3 pointerDragStarPosition, gearDragStarPosition;

    List<Axle> axlesInRange = new List<Axle>();

    public enum Type { Undefined, N1, S1, S2, S3 };

    Vector3 GetPointerWorldPosition()
    {
        Vector3 pointerPosition = Vector3.zero;
#if !UNITY_EDITOR
        if(Input.touchCount > 0)
            pointerPosition = Input.GetTouch(0).position;
#else
        pointerPosition = Input.mousePosition;
#endif
        return Camera.main.ScreenToWorldPoint(pointerPosition);
    }

    bool IsAvailableForUser
    {
        get => isAwailable && GameManager.Instance.animatedGearsCount == 0;
    }

    void OnMouseDown()
    {
        if (!IsAvailableForUser)
            return;
#if !UNITY_EDITOR
        if (Input.touchCount != 1)
            return;
#endif

        SoundsManager.Instance.Play(SoundsManager.ClipType.GearPickUp);
        pointerDragStarPosition = GetPointerWorldPosition();
        gearDragStarPosition = transform.position;
    }

    void OnMouseDrag()
    {
        if (!IsAvailableForUser)
            return;
#if !UNITY_EDITOR
        if (Input.touchCount != 1)
            return;
#endif
        var positionDelta = GetPointerWorldPosition() - pointerDragStarPosition;
        transform.position = gearDragStarPosition + positionDelta;
    }

    void OnMouseUp()
    {
        if (!IsAvailableForUser)
            return;

        void Reset()
        {
            SoundsManager.Instance.Play(SoundsManager.ClipType.GearPut);
            GameManager.Instance.ResetGear(this);
        };

        if (axlesInRange.Count != 0)
        {
            float minSqrDistance = float.MaxValue;
            Axle closestAxle = null;
            foreach (var axle in axlesInRange)
            {
                float sqrDistance = (transform.position - axle.transform.position).sqrMagnitude;
                if (minSqrDistance >= sqrDistance)
                {
                    closestAxle = axle;
                    minSqrDistance = sqrDistance;
                }
            }
            if (closestAxle != GameManager.Instance.GetCurrentAxle(this))
            {
                SoundsManager.Instance.Play(SoundsManager.ClipType.GearSwap);
                GameManager.Instance.SwapGears(this, closestAxle.currentGear);
                return;
            }
        }
        
        Reset();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var axle = collision.gameObject.GetComponent<Axle>();
        if (axle != null && !axlesInRange.Contains(axle))
            axlesInRange.Add(axle);
            
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var axle = collision.gameObject.GetComponent<Axle>();
        axlesInRange.Remove(axle);
    }
}
