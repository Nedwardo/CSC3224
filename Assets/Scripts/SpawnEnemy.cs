using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnEnemy : MonoBehaviour
{
    private BoxCollider2D mainCameraCollider;
    [SerializeField] private Tilemap map;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float durationToSpawnS;
    [SerializeField] private int maxEnemies;
    private float timeAtLastSpawn;
    private float currentTime;
    private GameObject Enemy;

    private int nonNullStart = -1;

    private int nonNullFinish = -1;

    public List<int>[] tilesByX; 
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
        // Enemies still sometimes spawn OOB IDK why, and at this point I'm just gonna blame unity and move on with my life
        Enemy = Instantiate(enemyPrefab, generateRandomSpawnPointOffScreen(), Quaternion.identity) as GameObject;
        Enemy.transform.parent = gameObject.transform;
    }

    private Vector2 generateRandomSpawnPointOffScreen(){
        Vector2 spawnLocation;
        int spawnWidth = 1 + nonNullFinish - nonNullStart;
        int leftCameraBoxCellSpace = map.WorldToCell(new Vector2(mainCameraCollider.bounds.center.x - mainCameraCollider.bounds.extents.x,0.0f)).x;
        int rightCameraBoxCellSpace = map.WorldToCell(new Vector2(mainCameraCollider.bounds.center.x + mainCameraCollider.bounds.extents.x,0.0f)).x;
        int cameraWidthCellSpace = rightCameraBoxCellSpace - leftCameraBoxCellSpace;
        int valueRange = spawnWidth - cameraWidthCellSpace;
        System.Random random = new System.Random();

        List<int> validY = new List<int>();
        int randomCellX = -1;

        float cellWidth =  new Vector3(map.CellToWorld(new Vector3Int(nonNullStart, 0, 0)).x - map.CellToWorld(new Vector3Int(nonNullStart+1, 0, 0)).x, 0.0f, 0.0f).x;

        
        while (validY.Count == 0){
            randomCellX = random.Next(valueRange) + nonNullStart;
            if (randomCellX > leftCameraBoxCellSpace){
                randomCellX += cameraWidthCellSpace;
            } 
            validY = tilesByX[randomCellX];
        }
        spawnLocation = map.CellToWorld(new Vector3Int(randomCellX, validY[random.Next(validY.Count)], 0));
        spawnLocation.y += 10;
        // I added this after testing, IDK why it needs it
        spawnLocation.x -= cellWidth;


        // int usedI;
        // for (int i = nonNullStart; i < nonNullFinish - cameraWidthCellSpace; i++){
        //     usedI = i;
        //     if (usedI > leftCameraBoxCellSpace){
        //         usedI += cameraWidthCellSpace;
        //     }
        //     validY = tilesByX[usedI];
        //     foreach (int Y in validY){
        //         Vector3 coordinate = map.CellToWorld(new Vector3Int(usedI, Y, 0));
        //         coordinate.y += 10;
        //         Debug.DrawLine(coordinate-(step/2), coordinate+(step/2), Color.cyan, int.MaxValue);
        //     }
        // }
        // Debug.Log("Spawned at :{" + spawnLocation.x + ", " + spawnLocation.y + "}");
        
        
        return spawnLocation;
    }

    private List<int>[] convertCompositeColliderToRanges(){
        BoundsInt bounds = map.cellBounds;
        TileBase[] allTiles = map.GetTilesBlock(bounds);
        List<int>[] tilesByX = new List<int>[bounds.size.x];
        for (int i = 0; i < bounds.size.x; i++){
            tilesByX[i] = new List<int>();
        }
        for (int y = 0; y < bounds.size.y; y++) {
            for (int x = 0; x < bounds.size.x; x++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    // -1 was added after extensive testing, I don't fully understand why I need it
                    tilesByX[x-1].Add(y);
                    // Vector2 aboveSelfCenter = map.CellToWorld(new Vector3Int(x-1, y, 0));
                    // Vector2 twiceAboveSelfCenter = map.CellToWorld(new Vector3Int(x-1, y+1, 0));
                    // Vector2 LeftSelfCenter = map.CellToWorld(new Vector3Int(x-2, y-1, 0));
                    // Vector2 RightSelfCenter = map.CellToWorld(new Vector3Int(x, y-1, 0));
                    // Vector2 aboveTopLeft = new Vector2 ((aboveSelfCenter.x + LeftSelfCenter.x)/2, (twiceAboveSelfCenter.y + aboveSelfCenter.y)/2);
                    // Vector2 aboveTopRight = new Vector2 ((aboveSelfCenter.x + RightSelfCenter.x)/2, (twiceAboveSelfCenter.y + aboveSelfCenter.y)/2);
                    // Vector2 aboveBottomLeft = new Vector2 ((aboveSelfCenter.x + LeftSelfCenter.x)/2, (LeftSelfCenter.y + aboveSelfCenter.y)/2);
                    // Vector2 aboveBottomRight = new Vector2 ((aboveSelfCenter.x + RightSelfCenter.x)/2, (LeftSelfCenter.y + aboveSelfCenter.y)/2);
                    // Debug.DrawLine(aboveTopLeft, aboveTopRight, Color.red, int.MaxValue);
                    // Debug.DrawLine(aboveTopRight, aboveBottomRight, Color.red, int.MaxValue);
                    // Debug.DrawLine(aboveBottomRight, aboveBottomLeft, Color.red, int.MaxValue);
                    // Debug.DrawLine(aboveBottomLeft, aboveTopLeft, Color.red, int.MaxValue);


                    // Vector2 visibleCoordinateLeft;
                    // Vector2 visibleCoordinateRight;
                    // visibleCoordinateLeft.x = (x-2)*10;
                    // visibleCoordinateRight.x = (x-1)*10;
                    // visibleCoordinateLeft.y = map.CellToWorld((Vector3Int) new Vector2Int(x, y)).y + 10;
                    // visibleCoordinateRight.y = map.CellToWorld((Vector3Int) new Vector2Int(x, y)).y + 10;
                    //Debug.DrawLine(visibleCoordinateLeft, visibleCoordinateRight, Color.yellow, int.MaxValue);
                }
            }
        }
        for (int i = 0; i < tilesByX.Length & (nonNullStart == -1 || nonNullFinish == -1); i++){
            if (tilesByX[i].Count != 0 && nonNullStart == -1){
                nonNullStart = i;
            }
            if (tilesByX[tilesByX.Length-i-1].Count != 0 && nonNullFinish == -1){
                nonNullFinish = tilesByX.Length-i-1;
            }
        }
        return tilesByX;
    }
}
