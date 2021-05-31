using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// [Serializable] public class BoolEvent : UnityEvent<bool> { }
public class MuseTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent TouchPressEvent;
    public UnityEvent TouchReleaseEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        TouchPressEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TouchReleaseEvent?.Invoke();
    }
}
