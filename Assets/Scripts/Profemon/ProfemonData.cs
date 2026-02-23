using UnityEngine;

[CreateAssetMenu(menuName = "Profemon/Professor")]
public class ProfemonData : ScriptableObject
{
    public string professorName;

    [TextArea(3, 5)]
    public string description;

    public Sprite image;

    [Range(0, 100)]
    public int captureDifficulty = 50;
}