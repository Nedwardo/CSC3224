using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D boundingBoxCollider;
    private BoxCollider2D mainCameraCollider;
    [SerializeField] private Tilemap map;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float durationToSpawnS;
    [SerializeField] private int maxEnemies;
    private float timeAtLastSpawn;
    private float currentTime;
    private GameObject Enemy;

    private List<float>[] tilesByX; 
    // Know calc width of map not in camera
    void Awake()
    {
        mainCameraCollider = GameObject.Find("MainCamera").GetComponent<BoxCollider2D>();
        tilesByX = convertCompositeColliderToRanges();
        timeAtLastSpawn = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time; 
        if(currentTime - timeAtLastSpawn > durationToSpawnS && gameObject.transform.childCount < maxEnemies){
            spawnEnemy();
            timeAtLastSpawn = Time.time;
        }
    }

    private void spawnEnemy(){
        Enemy = Instantiate(enemyPrefab, generateRandomSpawnPointOffScreen(), Quaternion.identity) as GameObject;
        Enemy.transform.parent = gameObject.transform;
    }

    private Vector2 generateRandomSpawnPointOffScreen(){
        // Vector2[] boundingPoints = boundingBoxCollider.points;
        // float[] boundingXCoordinates = new float[2];
        // boundingXCoordinates[0] = boundingPoints[0].x;
        // foreach (Vector2 point in boundingPoints){
        //     if (point.x != boundingXCoordinates[0]){
        //         boundingXCoordinates[1] = point.x;
        //         break;
        //     }
        // }
        // Debug.Log("Reachable (Spawn Enemy:33)");
        // float centerMapX = (boundingXCoordinates[0] + boundingXCoordinates[1])/2.0f;
        // float boxWidthX = System.Math.Abs(boundingXCoordinates[0] - boundingXCoordinates[1]);
        Vector2 spawnLocation;
        float centerMap = boundingBoxCollider.bounds.center.x;
        float boxWidth = boundingBoxCollider.bounds.extents.x;
        float centerCameraBox = mainCameraCollider.bounds.center.x;
        float centerCameraBoxWidth = mainCameraCollider.bounds.extents.x;

        float leftMapBox = centerMap - boxWidth;
        float leftCameraBox = centerCameraBox - centerCameraBoxWidth;
        float rangeOfValues = boxWidth*2 - centerCameraBoxWidth*2;
        System.Random random = new System.Random();


        spawnLocation.x = ((float) random.NextDouble() * rangeOfValues);
        spawnLocation.x += leftMapBox;
        if (spawnLocation.x > leftCameraBox){
            spawnLocation.x += centerCameraBoxWidth*2;
        }
        List<float> validY = convertXPosToYCoordinates(spawnLocation.x - leftMapBox);
        spawnLocation.y = validY[random.Next(validY.Count)];
        // Debug.Log("Spawned at :{" + spawnLocation.x + ", " + spawnLocation.y + "}");
        
        
        return spawnLocation;
    }

    private List<float>[] convertCompositeColliderToRanges(){
        BoundsInt bounds = map.cellBounds;
        TileBase[] allTiles = map.GetTilesBlock(bounds);
        List<float>[] tilesByX = new List<float>[bounds.size.x-2];
        for (int i = 0; i < bounds.size.x-2; i++){
            tilesByX[i] = new List<float>();
        }
        for (int y = 1; y < bounds.size.y-4; y++) {
            for (int x = 1; x < bounds.size.x-1; x++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null && allTiles[x + (y+1) * bounds.size.x] == null && allTiles[x+1 + (y+1) * bounds.size.x] == null && allTiles[x-1 + (y+1) * bounds.size.x] == null) {
                    // Vector2 visibleCoordinateLeft;
                    // Vector2 visibleCoordinateRight;
                    // visibleCoordinateLeft.x = (x-1)*2;
                    // visibleCoordinateRight.x = (x)*2;
                    // visibleCoordinateLeft.y = map.CellToWorld((Vector3Int) new Vector2Int(x-1, y)).y - 1;
                    // visibleCoordinateRight.y = map.CellToWorld((Vector3Int) new Vector2Int(x-1, y)).y - 1;
                    // Debug.DrawLine(visibleCoordinateLeft, visibleCoordinateRight, Color.red);
                    tilesByX[x-1].Add(map.CellToWorld((Vector3Int) new Vector2Int(x-1, y)).y - 1.0031f);
                }
            }
        }
        return tilesByX;
    }

    private List<float> convertXPosToYCoordinates(float x) {
        return tilesByX[((int) System.Math.Ceiling(x/2)) -1 ];
    }
}
