using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    float[] Blockage_Point = { -40f, -30f, -20f, -10f, 0, 10f, 20f, 30f, 40f,50f };

    //Multiple variant of blockages
    public GameObject[] Full_Block;
    public GameObject[] Jump_Block;
    public GameObject[] Dive_Block;

    public void BlockageGeneration()
    {
        //At each blockage point
        for (int i = 0; i < Blockage_Point.Length; i++)
        {
            bool passable = false;

            //Each Lane
            for (int lane = -1; lane <= 1; lane++)
            {
                //First Roll: Spawn blockage?
                int spawnBlock = Random.Range(0, 2);

                //if Spawning
                if (spawnBlock > 0)
                {
                    //Roll the type of the blockage that spawn here
                    int blockType = Random.Range(1, 4);
                    //1 = Jump, 2 = Dive, 3 = Full Block

                    if (blockType < 3) //if not a full block
                    { passable = true; } //This blockage point is ok - player is passible

                    //If rolled 3 full blockage, this point is not passable at all: reroll again with 0-2
                    if (blockType == 3 && lane == 1 && !passable)
                    { blockType = Random.Range(1, 3); }

                    //Spawn it
                    SpawnBlockage(blockType, lane, Blockage_Point[i]);
                }
                else //if not spawn: this point is passable
                { passable = true; }

            }
        }
    }

    void SpawnBlockage(int blockType, int lane, float spawnZ)
    {
        GameObject newBlock = null;

        //Spawn the blockage type
        switch (blockType)
        {
            case 1:
                int j = Random.Range(0, Jump_Block.Length);
                newBlock = Instantiate(Jump_Block[j], gameObject.transform);
                break;
            case 2:
                int d = Random.Range(0, Dive_Block.Length);
                newBlock = Instantiate(Dive_Block[d], gameObject.transform);
                break;
            case 3:
                int f = Random.Range(0, Full_Block.Length);
                newBlock = Instantiate(Full_Block[f], gameObject.transform);
                break;
        }

        //Spawn it onto the lane and ZPos
        newBlock.transform.localPosition = new Vector3(lane, newBlock.transform.position.y, spawnZ);
        



    }





}
