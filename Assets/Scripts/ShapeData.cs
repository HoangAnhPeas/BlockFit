using UnityEngine;

[CreateAssetMenu(fileName = "Shape", menuName = "Block Puzzle/Shape")]
public class ShapeData : ScriptableObject
{
    [Header("Shape")]
    public Vector2Int[] cells;

    [Header("Visual")]
    public Material material;

    [Header("Gameplay")]
    public int score = 10;
}