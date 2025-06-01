using UnityEngine;

[CreateAssetMenu(fileName = "NikkeData", menuName = "Scriptable Objects/NikkeData")]
public class NikkeData : ScriptableObject
{
    public string NikkeName;
    public GameObject NikkePrefab;
    public Sprite NikkeIcon;
}
