using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public AudioInteractivo audioGolpe;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            audioGolpe.ActivarAudio();
        }
    }
}
