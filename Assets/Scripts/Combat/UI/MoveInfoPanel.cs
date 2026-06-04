using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;

    [SerializeField] private TextMeshProUGUI moveName;
    [SerializeField] private TextMeshProUGUI power;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Background")]
    [SerializeField] private Image backgroundImage;

    [System.Serializable]
    public class TypeBackground
    {
        public TypeSO type;
        public Sprite sprite;
    }

    [SerializeField]
    private List<TypeBackground> backgrounds;

    private void Start()
    {
        Hide();
    }

    public void Show(MoveSO move)
    {
        root.SetActive(true);

        moveName.text = move.moveName;
        description.text = move.moveDescription;

        int movePower = move.power;

        power.text = movePower != 0 ? movePower.ToString() : "--";

        bool showAccuracy = move.accuracy > 0;

        accuracy.gameObject.SetActive(showAccuracy);

        if (showAccuracy)
            accuracy.text = move.accuracy + "%";

        Sprite bg = GetBackground(move.moveType);

        if (bg != null)
            backgroundImage.sprite = bg;

    }

    public void Hide()
    {
        root.SetActive(false);
    }

    Sprite GetBackground(TypeSO type)
    {
        foreach (var bg in backgrounds)
        {
            if (bg.type == type)
                return bg.sprite;
        }

        return null;
    }
}