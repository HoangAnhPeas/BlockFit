using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private Block blockPrefab;
    [SerializeField] private ShapeData[] shapes;

    [SerializeField] private Transform[] spawnPoints;

    private readonly List<Block> activeBlocks = new();

    void Start()
    {
        Debug.Log("BlockSpawner Start");
        SpawnNewSet();
    }

    public void SpawnNewSet()
    {
        activeBlocks.Clear();

        foreach (Transform point in spawnPoints)
        {
            Block block = Instantiate(blockPrefab, point.position, Quaternion.identity);

            block.Build(shapes[Random.Range(0, shapes.Length)]);

            block.OnPlaced += HandleBlockPlaced;

            activeBlocks.Add(block);
        }
    }

    void HandleBlockPlaced(Block block)
    {
        activeBlocks.Remove(block);

        if (activeBlocks.Count == 0)
            SpawnNewSet();
    }
}