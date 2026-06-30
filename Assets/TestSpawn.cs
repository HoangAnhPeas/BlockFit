using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private ShapeData shape;

    void Start()
    {
        Block block = Instantiate(blockPrefab);
        block.Build(shape);
    }
}