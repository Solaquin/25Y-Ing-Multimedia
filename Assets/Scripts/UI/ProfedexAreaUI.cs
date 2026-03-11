using UnityEngine;

public class ProfedexAreaUI : MonoBehaviour
{
    public ProfedexDatabase database;
    public TypeSO areaType;
    public Transform gridParent;
    public GameObject slotPrefab;

    public Sprite unknownSprite;

    void OnEnable()
    {
        GenerateProfemons();
    }

    void GenerateProfemons()
    {
        ClearGrid();

        foreach (var profemon in database.allProfemons)
        {
            if (profemon.types.Contains(areaType))
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