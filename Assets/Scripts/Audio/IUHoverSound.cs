using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIHoverGlobal : MonoBehaviour
{
    public AudioInteractivo audioUI;

    private GameObject ultimoHover;

    void Update()
    {
        if (EventSystem.current == null) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            GameObject objetoActual = results[0].gameObject;

            if (objetoActual != ultimoHover)
            {
                if (objetoActual.GetComponent<UnityEngine.UI.Selectable>() != null)
                {
                    if (audioUI != null)
                        audioUI.ActivarAudio();

                    ultimoHover = objetoActual;
                }
            }
        }
        else
        {
            ultimoHover = null;
        }
    }
}
