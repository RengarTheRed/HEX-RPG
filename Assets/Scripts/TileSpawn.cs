using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileSpawn : MonoBehaviour
{
    //Inspector Set Variables
    public int width, height, smoothRadius;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> tileTopper = new List<GameObject>();


    Tile[,] tileArray;

    // Start is called before the first frame update
    void Start()
    {
        tileArray = new Tile[width, height];
        SetFrameRate();
        GenerateTiles();
        RandomiseHeight();
        FlattenTerrain();
        GenerateTileTops();
    }
    void GenerateTiles()
    {
        for(int x=0; x < width; x++)
        {
            for(int z=0; z < height; z++)
            {
                //Spawns tile object then sets position
                GameObject newTile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Count)]);
                newTile.transform.position = new Vector3(x * GetXOffset()*.8f, 0, z * GetZOffset()*1.1f);

                //every other row add offset to align
                if(x%2 == 0)
                {
                    newTile.transform.Translate(new Vector3(0, 0, GetZOffset()/2f));
                }

                //Adds to array and parent
                newTile.transform.parent = this.transform;
                Tile newTileOb = new Tile(TileType.Plain, newTile);
                tileArray[x,z] = newTileOb;
            }
        }
    }
    float GetXOffset()
    {
        float offset = tilePrefabs[0].GetComponent<Renderer>().bounds.size.x;

        return offset;
    }
    float GetZOffset()
    {
        float offset = tilePrefabs[0].GetComponent<Renderer>().bounds.size.z;

        return offset;
    }

    //Function for smoothing terrain height
    void FlattenTerrain()
    {
        for (int x = 0; x < tileArray.GetLength(0); x++)
        {
            for(int z = 0; z < tileArray.GetLength(1); z++)
            {
                float averageHeight;
                int toDivideBy = 1;
                averageHeight = TileHeight(tileArray[x, z].TileObject());

                for (int i = 0; i < smoothRadius; i++)
                {
                    averageHeight += GetNearbyTileHeight(i, x, z, tileArray[x, z].TileObject());
                    toDivideBy++;
                }

                //Finally divide by amount of tiles weighted
                averageHeight = averageHeight / toDivideBy;
                tileArray[x,z].TileObject().transform.position = new Vector3(tileArray[x,z].TileObject().transform.position.x, averageHeight, tileArray[x, z].TileObject().transform.position.z);
            }
        }
    }
    void RandomiseHeight()
    {
        foreach(Tile tile in tileArray)
        {
            tile.TileObject().transform.position = new Vector3(tile.TileObject().transform.position.x, Random.Range(-2, 5), tile.TileObject().transform.position.z);
        }
    }
    float TileHeight(GameObject tileToGet)
    {
        return tileToGet.transform.position.y;
    }
    float GetNearbyTileHeight(int max, int x, int z, GameObject tile)
    {
        float averageHeight = tile.transform.position.y;
        int toDivide = 1;

        //Lows
        if (x-max > 0)
        {
            averageHeight += TileHeight(tileArray[x - max - 1, z].TileObject());
            toDivide++;
        }
        if (z-max > 0)
        {
            averageHeight += TileHeight(tileArray[x, z - max - 1].TileObject());
            toDivide++;
        }

        //Highs
        if (x + max < tileArray.GetLength(0) - 1)
        {
            averageHeight += TileHeight(tileArray[x + max + 1, z].TileObject());
            toDivide++;
        }
        if (z + max < tileArray.GetLength(1) - 1)
        {
            averageHeight += TileHeight(tileArray[x, z + max + 1].TileObject());
            toDivide++;
        }

        averageHeight = averageHeight / toDivide;
        return averageHeight;
    }
    void SetFrameRate()
    {
        Application.targetFrameRate = 60;
    }
    void GenerateTileTops()
    {
        foreach (Tile tile in tileArray)
        {
            //Adds tile topper
            int topIndex = Random.Range(0, tileTopper.Count);
            GameObject tileTop = Instantiate(tileTopper[topIndex]);

            tileTop.transform.position = new Vector3(tile.TileObject().transform.position.x, tile.TileObject().transform.position.y + tile.TileObject().GetComponent<MeshFilter>().sharedMesh.bounds.max.y + tileTop.GetComponent<Collider>().bounds.size.y, tile.TileObject().transform.position.z);

            tileTop.transform.parent = tile.TileObject().transform;
        }
        Debug.Log("Tops generated");
    }
}
class HeightCalculator
{
    public int toDivide;
    public float averageHeight;
    public HeightCalculator(int div, float height)
    {
        toDivide = div;
        averageHeight = height;
    }
}

class Tile
{
    TileType tileType;
    GameObject tileObject;
    GameObject tileTopper;
    public Tile(TileType nTileType, GameObject tObject)
    {
        tileType = nTileType;
        tileObject = tObject;
        SpawnTileTop();
    }
    public GameObject TileObject()
    {
        return tileObject;
    }
    public void ChangeTileTop(TileType newType)
    {

        tileType = newType;
    }

    void SpawnTileTop()
    {
        //GETS SPAWNSCRIPT ACCESS HERE
        TileSpawn tileSpawnScript = tileObject.GetComponentInParent<TileSpawn>();

    }
}
enum TileType
{
    Plain,
    Path,
    Forest
}