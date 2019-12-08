using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
   
    [HideInInspector] public bool Gameover;
    [HideInInspector] public int Score;
    [HideInInspector] public float Difficulty;

    //UI
    public Camera MenuCamera;
    public Text Title_Text;
    public Text Intro_Text;
    public Button Play_Button;
    public Text Score_Text;
    public Text Gameover_Text;
    public Button Retry_Button;
    public Button Exit_Button;

    public GameObject Player_Prefab;
    private GameObject PlayerInScene = null;
    public GameObject Field_Prefab;
    private Vector3 Field_Spawn_Pos = new Vector3(0,0,0);

    private void Awake()
    {
        Screen.SetResolution(540, 960, false);
        instance = GetComponent<GameController>();
    }

    public void StartGame()
    {
        //Clear UI
        Title_Text.gameObject.SetActive(false);
        Intro_Text.gameObject.SetActive(false);
        Play_Button.gameObject.SetActive(false);
        MenuCamera.gameObject.SetActive(false);

        //Set parameters
        Gameover = false;
        Score = 0;
        Score_Text.gameObject.SetActive(true);
        Difficulty = 0;
        Exit_Button.gameObject.SetActive(true);

        //Spawn the two initial field
        SpawnField();
        SpawnField();

        //Spawn Player
        PlayerInScene = Instantiate(Player_Prefab);

        StartCoroutine(GameLoop());
    }

    public void RestartGame()
    {
        Destroy(PlayerInScene);
        foreach (GameObject usedField in GameObject.FindGameObjectsWithTag("Field"))
        { Destroy(usedField);}

        Gameover_Text.gameObject.SetActive(false);
        Retry_Button.gameObject.SetActive(false);
        Field_Spawn_Pos.z = 0;
        StartGame();
    }

    public void BackToMenu()
    {
        Destroy(PlayerInScene);
        foreach (GameObject usedField in GameObject.FindGameObjectsWithTag("Field"))
        { Destroy(usedField); }

        Score_Text.gameObject.SetActive(false);
        Gameover_Text.gameObject.SetActive(false);
        Retry_Button.gameObject.SetActive(false);
        Field_Spawn_Pos.z = 0;
        Exit_Button.gameObject.SetActive(false);

        //Menu UI
        MenuCamera.gameObject.SetActive(true);
        Title_Text.gameObject.SetActive(true);
        Intro_Text.gameObject.SetActive(true);
        Play_Button.gameObject.SetActive(true);
    }

    IEnumerator GameLoop()
    {
        while (!Gameover)
        {
            //+1 Score per second
            Score++;
            if (Score >= 100)
            {
                //Each 100 score will rise the difficulty
                int diffIncrease = Score / 100;
                Difficulty = 0.012f * diffIncrease;
            }
            UpdateScore();
            yield return new WaitForSeconds(0.5f);
        }

        //When gameover
        Gameover_Text.text = "Game over\nYour score is\n" + Score;
        Gameover_Text.gameObject.SetActive(true);
        Retry_Button.gameObject.SetActive(true);

    }

    public void UpdateScore()
    {
        //Refresh the score on UI
        Score_Text.text = "Score: " + Score;
    }


    public void CheckPointReached(GameObject fieldPassed)
    {
        //Spawn next part of the field
        SpawnField();

        //Kill the useless part of field
        Destroy(fieldPassed, 1f);
    }

    void SpawnField()
    {
        GameObject newField = Instantiate(Field_Prefab);
        newField.transform.position = Field_Spawn_Pos;
        newField.GetComponent<Field>().BlockageGeneration();
        Field_Spawn_Pos.z += 100f;
    }



}
