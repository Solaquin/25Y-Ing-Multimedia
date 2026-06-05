using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowProfemonInfo : MonoBehaviour
{

    [SerializeField] private Profemon profemon;
    private ProfemonData data;
    private TypeSO typeData;

    public TextMeshProUGUI profemonName;
    public Image profemonImage;

    public TextMeshProUGUI areaName;
    public Image areaIcon;

    void Start()
    {
        data = profemon.data;
        typeData = data.types[0];

        profemonName.text = data.professorName;
        profemonImage.sprite = data.image;
        
        areaName.text = typeData.shortName;
        areaIcon.sprite = typeData.icon;
    }
}
