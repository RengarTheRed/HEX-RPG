using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileSpawn : MonoBehaviour
{

    public int width, height;
    public List<GameObject> tilePrefabs = new List<GameObject>();
    public List<GameObject> tileTopper = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateTiles();
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

                //Adds to parent
                tiles.Add(newTile);
                newTile.transform.parent = this.transform;

                //Adds tile topper
                GameObject tileTop = Instantiate(tileTopper[Random.Range(0, tileTopper.Count)]);
                tileTop.transform.position = new Vector3(newTile.transform.position.x, newTile.GetComponent<MeshFilter>().sharedMesh.bounds.max.y+tileTop.GetComponent<Renderer>().bounds.size.y, newTile.transform.position.z);

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
