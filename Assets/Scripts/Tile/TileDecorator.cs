using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDecorator : MonoBehaviour
{
    [SerializeField] private GameObject[] rockDecorations;
    [SerializeField] private GameObject[] grassDecorations;
    [SerializeField] private float rockGrassRatio; // 1 = alleen rocks en 0 = alleen grass
    [SerializeField] private float decorationSpawnHeight;
    [SerializeField] private int minSpawns;
    [SerializeField] private int maxSpawns;
    [SerializeField] private float maxSpawnOffset;
    //[SerializeField] private float minSpawnDistance;

    private Vector3 decorationPosition;
    private Quaternion decorationRotation;
    private List<Vector3> decorationPositions = new List<Vector3>();

    void Start()
    {
        for (int b = 0; b < Random.Range(minSpawns, maxSpawns); b++)
        {
            SpawnDecoration();
        }
    }

    void SpawnDecoration()
    {
        decorationPosition = transform.position + new Vector3(Random.Range(-maxSpawnOffset, maxSpawnOffset), decorationSpawnHeight, Random.Range(-maxSpawnOffset, maxSpawnOffset));
        decorationRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + Random.Range(-360, 360), transform.rotation.z);

        /*
        foreach (Vector3 pos in decorationPositions)
        {
            if (Vector3.Distance(decorationPosition, pos) < minSpawnDistance)
            {
                SpawnDecoration();
                break;
            }
        }
        */

        GameObject newDecoration;

        if (Random.Range(0.0f, 1.0f) < rockGrassRatio)
        {
            newDecoration = Instantiate(rockDecorations[Random.Range(0, rockDecorations.Length)], decorationPosition, decorationRotation);
        } else
        {
            newDecoration = Instantiate(grassDecorations[Random.Range(0, grassDecorations.Length)], decorationPosition, decorationRotation);
        }
        
        newDecoration.transform.parent = this.transform;
        decorationPositions.Add(decorationPosition);
    }
}
