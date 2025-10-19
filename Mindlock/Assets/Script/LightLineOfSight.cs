using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal; // pokud používáš Light2D

public class LightLineOfSightTilemap : MonoBehaviour
{
    public Transform player;
    public Light2D light2D;
    public Tilemap wallTilemap; // Tilemap se zdmi

    void Update()
    {
        if (player == null || light2D == null || wallTilemap == null) return;

        Vector3 start = transform.position;
        Vector3 end = player.position;
        Vector3 dir = end - start;
        float distance = dir.magnitude;

        // Počítej krok po kroku mezi startem a koncem
        int steps = Mathf.CeilToInt(distance / wallTilemap.cellSize.x);
        bool blocked = false;

        for (int i = 0; i <= steps; i++)
        {
            Vector3 pos = start + dir * (i / (float)steps);
            Vector3Int cellPos = wallTilemap.WorldToCell(pos);

            if (wallTilemap.HasTile(cellPos))
            {
                blocked = true;
                break;
            }
        }

        light2D.enabled = !blocked;
    }
}
