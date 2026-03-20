using UnityEngine;

public class StorageMenuManager : MonoBehaviour
{
    public Transform partyGrid;
    public Transform storageGrid;

    public GameObject slotPrefab;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform t in partyGrid)
            Destroy(t.gameObject);

        foreach (Transform t in storageGrid)
            Destroy(t.gameObject);

        foreach (var p in PlayerPartyManager.Instance.party)
        {
            GameObject slot = Instantiate(slotPrefab, partyGrid);
            slot.GetComponent<StorageSlot>().Setup(p);
        }

        foreach (var s in PlayerPartyManager.Instance.storage)
        {
            GameObject slot = Instantiate(slotPrefab, storageGrid);
            slot.GetComponent<StorageSlot>().Setup(s);
        }
    }
}