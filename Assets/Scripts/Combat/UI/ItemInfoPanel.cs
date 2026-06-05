using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    private void Start()
    {
        Hide();
    }

    public void Show(ItemSO item)
    {
        description.text = item.description;
    }

    public void Hide()
    {
        description.text = "--";
    }
}