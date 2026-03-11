using UnityEngine;
using System.Collections.Generic;

public class ProfedexUI : MonoBehaviour
{
    public ProfedexDatabase database;

    public Transform gridParent;
    public GameObject slotPrefab;

    public Sprite unknownSprite;

    public void ShowType(TypeSO type)
    {
        ClearGrid();

        foreach (var profemon in database.allProfemons)
        {
            if (profemon.types.Contains(type))
            {
                GameObject slot = Instantiate(slotPrefab, gridParent);

                bool discovered = ProfedexManager.Instance.IsRegistered(profemon);

                slot.GetComponent<ProfedexSlot>()
                    .Setup(profemon, discovered, unknownSprite);
            }
        }
    }

    void ClearGrid()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }
}