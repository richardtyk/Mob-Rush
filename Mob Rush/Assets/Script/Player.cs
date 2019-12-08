using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Effect
    public GameObject DiveEffect;

    //Sound Effect
    private AudioSource sfxPlayer;
    public AudioClip JumpSound;
    public AudioClip DiveSound;
    public AudioClip DeathSound;

    private float Movement_Speed = 0.12f;
    private float Jump_Height = 1.6f;

    //Prevent interupting action
    private bool Action_Restricted = false;

    //0 = Mid, 1 = Right, -1 = Left (X Pos)
    private int Current_Lane = 0;

    private bool Alive;

    // Start is called before the first frame update
    void Start()
    {
        Alive = true;
        sfxPlayer = GetComponent<AudioSource>();
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        while (Alive)
        {
            //Player is always moving forward (Moving along Z)
            transform.position += transform.forward * Time.deltaTime * Movement_Speed;
            transform.Translate(Vector3.forward * (Movement_Speed + GameController.instance.Difficulty));

            //Trying to switch left
            if (Input.GetKeyDown(KeyCode.LeftArrow) && Current_Lane != -1 && !Action_Restricted)
            {
                Action_Restricted = true;
                StartCoroutine(Switching("L"));
            }
            //Trying to switch right
            if (Input.GetKeyDown(KeyCode.RightArrow) && Current_Lane != 1 && !Action_Restricted)
            {
                Action_Restricted = true;
                StartCoroutine(Switching("R"));
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && !Action_Restricted)
            {
                Action_Restricted = true;
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && !Action_Restricted)
            {
                Action_Restricted = true;
                StartCoroutine(Dive());
            }

            yield return null;
        }
    }

    IEnumerator Switching(string toLane)
    {
        switch (toLane)
        {
            case "L":
                float destLaneL = Current_Lane - 1;
                while (transform.position.x > destLaneL)
                {
                    transform.Translate(Vector3.left * 0.1f);
                    yield return new WaitForSeconds(0.01f);
                }
                //Movement complete, set to correct position
                Current_Lane -= 1;
                transform.position = new Vector3(Current_Lane, transform.position.y, transform.position.z);
                break;
            case "R":
                float destLaneR = Current_Lane + 1;
                while (transform.position.x < destLaneR)
                {
                    transform.Translate(Vector3.right * 0.1f);
                    yield return new WaitForSeconds(0.01f);
                }
                Current_Lane += 1;
                transform.position = new Vector3(Current_Lane, transform.position.y, transform.position.z);
                break;
        }

        //Switch is complete, Player can now switch lane again
        if (Alive) //just to prevent bug as there is a chance that player died while mid-switch
        { Action_Restricted = false; }

    }

    IEnumerator Jump()
    {
        sfxPlayer.clip = JumpSound;
        sfxPlayer.Play();
        GetComponent<Animator>().SetBool("Run", false);
        GetComponent<Animator>().SetTrigger("Jump");

        //Jumping upward
        while (transform.position.y < Jump_Height)
        {
            transform.Translate(Vector3.up * 0.1f);
            yield return new WaitForSeconds(0.01f);
        }

        //Slight hold in mid air for easier jumping
        yield return new WaitForSeconds(0.25f);

        //Falling back to ground
        while (transform.position.y > 0.5)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(Vector3.down * 0.1f);
        }
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        //Jump complete
        if (Alive) //just to prevent bug as there is a chance that player died while mid-air
        {
            Action_Restricted = false;
            GetComponent<Animator>().SetBool("Run", true);
        }   
    }

    IEnumerator Dive()
    {
        sfxPlayer.clip = DiveSound;
        sfxPlayer.Play();
        DiveEffect.SetActive(true);

        //Dive downward
        while (transform.position.y > 0.3)
        {
            transform.Translate(Vector3.down * 0.025f);
            yield return new WaitForSeconds(0.01f);
        }

        //Slight hold in underground
        yield return new WaitForSeconds(0.55f);

        //Going back up
        while (transform.position.y < 0.5)
        {
            yield return new WaitForSeconds(0.0f);
            transform.Translate(Vector3.up * 0.025f);
        }
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        //Dive complete
        DiveEffect.SetActive(false);
        if (Alive) //just to prevent bug as there is a chance that player died while underground
        { Action_Restricted = false; }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If hitting a check point
        if (other.tag == "CP")
        {
            Debug.Log("CP reached");
            GameController.instance.CheckPointReached(other.transform.parent.gameObject);
        }
        //else it can only be something u dont want to hit
        else if (other.tag == "Blockage")
        {
            Debug.Log("Blockagehit!");
            Alive = false;
            Action_Restricted = true;
            sfxPlayer.clip = DeathSound;
            sfxPlayer.Play();

            GameController.instance.Gameover = true;

            //Set animator state into dead until player revive
            GetComponent<Animator>().SetBool("Run",false);
            GetComponent<Animator>().SetTrigger("Die");

        }
    }

}
