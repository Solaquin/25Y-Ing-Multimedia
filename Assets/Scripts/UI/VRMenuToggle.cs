using UnityEngine;

public class VRMenuToggle : MonoBehaviour
{
    public GameObject menuRoot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            menuRoot.SetActive(!menuRoot.activeSelf);
        }
    }
}