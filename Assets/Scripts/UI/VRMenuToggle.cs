using UnityEngine;

public class VRMenuToggle : MonoBehaviour
{
    public GameObject menuRoot;
    public VRMenuManager menuManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool isActive = menuRoot.activeSelf;

            menuRoot.SetActive(!isActive);

            if (!isActive)
            {
                menuManager.ResetMenu();
            }
        }
    }
}