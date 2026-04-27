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

        GameObject objetoActual = null;

        // FUNCIONA PARA AMBOS (MOUSE Y XR)
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        // esto permite que XR también funcione
        pointerData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            objetoActual = results[0].gameObject;
        }

        // SONIDO SOLO SI CAMBIA EL HOVER
        if (objetoActual != null)
        {
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