using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject tileObj;
    public string type;
    public Tile(GameObject tileObj, string type)
    {
        tileObj = this.tileObj;
        type = this.type;
    }
}

public class CreateGame : MonoBehaviour {

    GameObject tile1 = null;
    GameObject tile2 = null;
    List<GameObject> tileBank = new List<GameObject>();
    static int rows = 9;
    static int cols = 6;
    Tile[,] tiles = new Tile[cols, rows];
    bool renewBoard = false;

    public GameObject[] tile;

    // Use this for initialization
    void Start () {
        // create a tileBank
        int numCopies = (rows * cols) / 3;
        for (int i = 0; i < numCopies; i++)
        {
            for (int j = 0; j < tile.Length; j++)
            {
                GameObject o = (GameObject)Instantiate(tile[j],
                    new Vector3(-10, -10, 0),
                    tile[j].transform.rotation);
                o.SetActive(false);
                tileBank.Add(o);
            }
        }

        // randomize tiles
        ShuffleList();

		// initialize tile grid
        for (int r = 0; r < rows; r++)
        {
            for(int c = 0; c < cols; c++)
            {
                Vector3 tilePos = new Vector3(c, r, 0);
                for (int n = 0; n < tileBank.Count; n++)
                {
                    GameObject o = tileBank[n];
                    if (!o.activeSelf)
                    {
                        o.transform.position = new Vector3(tilePos.x, tilePos.y, tilePos.z);
                        o.SetActive(true);
                        tiles[c, r] = new Tile(o, o.name);
                        n = tileBank.Count + 1;
                    }
                }
            }
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        CheckGrid();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);
            if(hit)
            {
                tile1 = hit.collider.gameObject;
            }
        }

        // if finger up is detected after an initial tiel has been chosen
        else if( Input.GetMouseButtonUp(0) && tile1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);
            if (hit)
            {
                tile2 = hit.collider.gameObject;
            }
            if(tile1 && tile2)
            {
                // check tile distances between selected tiles
                int horzDist = (int)Mathf.Abs(tile1.transform.position.x - tile2.transform.position.x);
                int vertDist = (int)Mathf.Abs(tile1.transform.position.y - tile2.transform.position.y);
                
                // only swap if they are 1 away
                //TODO FIX THIS SO YOU CAN'T SWAP 1,2 OR 2,1, ETC
                if (horzDist == 1 ^ vertDist == 1)
                {
                    // swap the tiles in memory (tiles[,]) for matching logic
                    Tile temp = tiles[(int)tile1.transform.position.x,
                        (int)tile1.transform.position.y];
                    tiles[(int)tile1.transform.position.x, (int)tile1.transform.position.y] =
                        tiles[(int)tile2.transform.position.x, (int)tile2.transform.position.y];
                    tiles[(int)tile2.transform.position.x, (int)tile2.transform.position.y] = temp;

                    // swap the tiles in the game on screen
                    Vector3 tempPos = tile1.transform.position;
                    tile1.transform.position = tile2.transform.position;
                    tile2.transform.position = tempPos;

                    // reset touched tiles
                    tile1 = null;
                    tile2 = null;
                }
                else
                {
                    GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    // Shuffles tiles so they aren't in the same order each game
    void ShuffleList()
    {
        System.Random rand = new System.Random();
        int r = tileBank.Count;
        while (r > 1)
        {
            r--;
            int n = rand.Next(r + 1);
            GameObject val = tileBank[n];
            tileBank[n] = tileBank[r];
            tileBank[r] = val;
        }
    }

    // checks grid for matches and removes them
    void CheckGrid()
    {
        int counter = 1;

        //check columns first
        for(int r = 0; r < rows; r++)
        {
            counter = 1;
            for (int c = 1; c < cols; c++)
            {
               // if the tiles exists then we compare
                if (tiles[c, r] != null && tiles[c -1, r] != null)
                {
                    if (tiles[c, r].type == tiles[c - 1, r].type)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 1; // reset counter
                    }
                    // remove if 3 are found
                    if (counter == 3) 
                    {
                        if (tiles[c, r] != null)
                            Debug.Log("c = " + c + " r = " + r);
                            tiles[c, r].tileObj.SetActive(false);
                        if (tiles[c - 1, r] != null)
                            tiles[c - 1, r].tileObj.SetActive(false);
                        if (tiles[c - 2, r] != null)
                            tiles[c - 2, r].tileObj.SetActive(false);
                        tiles[c, r] = null;
                        tiles[c - 1, r] = null;
                        tiles[c - 2, r] = null;
                        renewBoard = true;

                    }
                }
            }
        }
        //check rows next
        for (int c = 0; c < cols; c++)
        {
            counter = 1;
            for (int r = 1; r < rows; r++)
            {
                // if the tiles exists then we compare
                if (tiles[c, r] != null && tiles[c, r - 1] != null)
                {
                    if (tiles[c, r].type == tiles[c, r - 1].type)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 1; // reset counter
                    }
                    // remove if 3 are found
                    if (counter == 3)
                    {
                        if (tiles[c, r] != null)
                            tiles[c, r].tileObj.SetActive(false);
                        if (tiles[c, r - 1] != null)
                            tiles[c, r - 1].tileObj.SetActive(false);
                        if (tiles[c, r - 2] != null)
                            tiles[c, r - 2].tileObj.SetActive(false);
                        tiles[c, r] = null;
                        tiles[c, r - 1] = null;
                        tiles[c, r - 2] = null;
                        renewBoard = true;
                    }
                }
            }
        }
        if(renewBoard)
        {
            //RenewGrid();
            //renewBoard = false;
        }
    }
}
