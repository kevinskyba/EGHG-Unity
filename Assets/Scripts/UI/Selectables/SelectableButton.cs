
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KevinSkyba.EGHG.UI.Selectables
{
    public class SelectableButton : Selectable
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void EGHGOnStartFocus()
        {
            if (button && button.interactable)
            {
                button.OnPointerEnter(null);
                button.image.color = button.colors.highlightedColor;
            }
        }

        public void EGHGOnEndFocus()
        {
            if (button && button.interactable)
            {
                button.OnPointerExit(null);
                button.image.color = button.colors.normalColor;
            }
        }

        public void EGHGOnSelection()
        {
            if (button && button.interactable)
            {
                button.OnPointerClick(new PointerEventData(EventSystem.current));
                button.image.color = button.colors.pressedColor;
                StartCoroutine(DelayedSelectionRecover());
            }
        }

        private IEnumerator DelayedSelectionRecover()
        {
            yield return new WaitForSeconds(0.25f);
            if (button.image.color == button.colors.pressedColor)
            {
                button.image.color = button.colors.normalColor;
            }
        }
    }
}