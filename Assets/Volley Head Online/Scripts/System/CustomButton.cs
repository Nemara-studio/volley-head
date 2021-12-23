using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VollyHead.Online
{
    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsPressed = false;
        public UnityEvent onPressed = new UnityEvent();
        public UnityEvent onReleased = new UnityEvent();

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            onPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            onReleased?.Invoke();
        }

        private void OnDisable()
        {
            IsPressed = false;
            onReleased?.Invoke();
        }
    }
}