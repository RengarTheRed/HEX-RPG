using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileSpawn : MonoBehaviour
{

    public int width, height, smoothRadius;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> tileTopper = new List<GameObject>();
    GameObject[,] tileArray;
    // Start is called before the first frame update
    void Start()
    {
        tileArray = new GameObject[width, height];
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
                tileArray[x,z] = newTile;
                newTile.transform.parent = this.transform;
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

                averageHeight = TileHeight(tileArray[x, z]);

                for (int i = 0; i < smoothRadius; i++)
                {
                    averageHeight += GetNearbyTileHeight(i, x, z, tileArray[x, z]);
                    toDivideBy++;
                }

                //Finally divide by amount of tiles weighted
                averageHeight = averageHeight / toDivideBy;
                tileArray[x,z].transform.position = new Vector3(tileArray[x,z].transform.position.x, averageHeight, tileArray[x, z].transform.position.z);
            }
        }
    }
    void RandomiseHeight()
    {
        foreach(GameObject tile in tileArray)
        {
            tile.transform.position = new Vector3(tile.transform.position.x, Random.Range(-2, 5), tile.transform.position.z);
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
            averageHeight += TileHeight(tileArray[x - max - 1, z]);
            toDivide++;
        }
        if (z-max > 0)
        {
            averageHeight += TileHeight(tileArray[x, z - max - 1]);
            toDivide++;
        }

        //Highs
        if (x + max < tileArray.GetLength(0) - 1)
        {
            averageHeight += TileHeight(tileArray[x + max + 1, z]);
            toDivide++;
        }
        if (z + max < tileArray.GetLength(1) - 1)
        {
            averageHeight += TileHeight(tileArray[x, z + max + 1]);
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
        foreach (GameObject tile in tileArray)
        {
            //Adds tile topper
            int topIndex = Random.Range(0, tileTopper.Count);
            GameObject tileTop = Instantiate(tileTopper[topIndex]);

            tileTop.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + tile.GetComponent<MeshFilter>().sharedMesh.bounds.max.y + tileTop.GetComponent<Collider>().bounds.size.y, tile.transform.position.z);

            tileTop.transform.parent = tile.transform;
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