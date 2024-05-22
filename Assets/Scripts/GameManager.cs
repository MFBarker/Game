using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class GameManager : Singleton<GameManager>
{
    [Header("Player Info")]
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] IntVariable lives;
    [SerializeField] IntVariable score;
    [SerializeField] FloatVariable health;
    [SerializeField] FloatVariable timer;

    [SerializeField] GameObject respawn;

    [Header("Events")]
    [SerializeField] public VoidEvent gameStartEvent;
    [SerializeField] public GameObjectEvent respawnEvent;

    [Header("Runes")]
    [SerializeField] GameObject Rune_N;
    [SerializeField] GameObject Rune_S;
    [SerializeField] GameObject Rune_E;
    [SerializeField] GameObject Rune_W;

    [Header("Door")]
    [SerializeField] GameObject Gate_Closed;
    [SerializeField] GameObject Gate_Open;

    [Header("UI")]
    [SerializeField] GameObject Win;
    [SerializeField] GameObject Lose;
    [SerializeField] GameObject LoseMoonshine;
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject Hint;

    [Header("Audio")]
    [SerializeField] AudioSource Title_Theme;
    [SerializeField] AudioSource Main_Theme;
    [SerializeField] AudioSource Win_Theme;
    [SerializeField] AudioSource Loss_Theme;

    [Header("SFX")]
    [SerializeField] AudioSource doorOpenSFX;
    enum State
    { 
        TITLE,
        TUTORIAL,
        START_GAME,
        PLAY_GAME,
        GAME_WIN,
        GAME_OVER
    }
    public bool canMove { get; set; } = false;
    public bool statueActivated { get; set; } = false;
    public bool pillarWestActivated { get; set; } = false;
    public bool pillarEastActivated { get; set; } = false;
    public bool pillarNorthActivated { get; set; } = false;
    public bool doorOpen { get; set; } = false;
    public bool allRunes { get; set; } = false;

    //items lol
    public bool drankTheMoonshine { get; set; } = false;

    private State state = State.TITLE;
    // Start is called before the first frame update

    private List<GameObject> itemList = new List<GameObject>();

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        switch (state)
        {
            case State.TITLE:
                if (Title_Theme.isPlaying == false)
                { 
                    Title_Theme.Play();
                }
                UIManager.Instance.SetActive("Title", true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                break;

            case State.START_GAME:
                UIManager.Instance.SetActive("Title", false);
                // reset values
                timer.value = 300;
                lives.value = 3;
                health.value = 100;

                //add items
                SpawnItem.Instance.PlaceItem();

                gameStartEvent.RaiseEvent();
                respawnEvent.RaiseEvent(respawn);

                canMove = true;
                 
                state = State.TUTORIAL;
                Title_Theme.Stop();
                break;
            case State.TUTORIAL:
                if (Main_Theme.isPlaying == false) Main_Theme.Play();
                //show controls
                Controls.SetActive(true);
                break;
            case State.PLAY_GAME:
                if(Main_Theme.isPlaying == false) Main_Theme.Play();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //hide controls
                Controls.SetActive(false);
                //show or hide hint
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (Hint.activeSelf == false) Hint.SetActive(true);
                    else Hint.SetActive(false);
                }
                // game timer
                timer.value = timer - Time.deltaTime;
                if (timer <= 0)
                {
                    canMove = false;
                    state = State.GAME_OVER;
                }
                if (health <= 0)
                {
                    canMove = false;
                    state = State.GAME_OVER;
                }

                CheckRunes();

                break;
            case State.GAME_WIN:
                if (Win_Theme.isPlaying == false) Win_Theme.Play();
                if (Main_Theme.isPlaying == true) Main_Theme.Stop();
                Win.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                break;
            case State.GAME_OVER:
                if(Loss_Theme.isPlaying == false) Loss_Theme.Play();
                if (Main_Theme.isPlaying == true) Main_Theme.Stop();
                if (drankTheMoonshine) LoseMoonshine.SetActive(true);
                else Lose.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                break;
            default:
                break;
        }
        UIManager.Instance.Timer = timer;
    }

    #region Game Logic
    public void OnStartGame()
    {
        state = State.START_GAME;
        print("Start_game");
    }
    public void OnPlayGame()
    {
        print("Play Game");
        state = State.PLAY_GAME;
    }
    /// <summary>
    /// Resets the runes, clears out any items, hides the win/lose screen, 
    /// and returns to the title screen.
    /// </summary>
    public void BackToTitle()
    {
        ResetRunes();

        itemList.Clear();

        if (Win.activeSelf == true)
        { 
            Win.SetActive(false);
            Win_Theme.Stop();
        }
        if(Lose.activeSelf == true || LoseMoonshine.activeSelf == true)
        {
            if (drankTheMoonshine == true) 
            {
                drankTheMoonshine = false;
                LoseMoonshine.SetActive(false);
            }
            else Lose.SetActive(false);
            Loss_Theme.Stop();
        }
        state = State.TITLE;
    }
    public void OnAddPoints(int points)
    {
        print(points);
        score.value += points;
    }
    #endregion

    #region Runes_Methods
    /// <summary>
    /// Resets the runes and win conditions for the game.
    /// </summary>
    public void ResetRunes()
    {
        Rune_S.SetActive(false);
        Rune_N.SetActive(false);
        Rune_E.SetActive(false);
        Rune_W.SetActive(false);

        statueActivated = false;
        pillarEastActivated = false;
        pillarWestActivated = false;
        pillarNorthActivated = false;

        doorOpen = false;
        Gate_Closed.SetActive(true);
        Gate_Open.SetActive(false);

        allRunes = false;
    }

    /// <summary>
    /// Checks if all runes have been activated, and progress is made.
    /// </summary>
    public void CheckRunes()
    {
        //Activate Altar
        if (statueActivated == true && Rune_S.activeSelf == false)
        {
            Rune_S.SetActive(true); 
        }
        else if (pillarWestActivated == true && Rune_W.activeSelf == false)
        {
            Rune_W.SetActive(true);
        }
        else if (pillarEastActivated == true && Rune_E.activeSelf == false)
        {
            Rune_E.SetActive(true);
        }
        else if (pillarNorthActivated == true && Rune_N.activeSelf == false)
        {
            Rune_N.SetActive(true);
            allRunes = true;
        }

        //Check for door open
        if (statueActivated == true && pillarWestActivated == true && pillarEastActivated == true && doorOpen == false)
        {
            doorOpen = true;
            Gate_Closed.SetActive(false);
            Gate_Open.SetActive(true);
            doorOpenSFX.Play();
        }

        //Check for all Runes (Game Win)
        if(allRunes == true) 
        {
            //Game Win
            print("Game Win");
            state = State.GAME_WIN;
        }
    }

    public void PillarActivate(string type, GameObject glow)
    {
        glow.SetActive(true);

        print($"{type} was activated");
    }
    #endregion 

    #region Items_Methods
    /// <summary>
    /// Instantly kills the player. Will only be called if the player drinks moonshine.
    /// </summary>
    public void KillPlayer()
    {
        //instant kill
        health.value -= 9999;
        drankTheMoonshine = true;
    }

    /// <summary>
    /// Subracts time from the timer.
    /// </summary>
    /// <param name="value">Amount of time to remove</param>
    public void TimeReduction(float value)
    {
        timer.value -= value;
    }

    /// <summary>
    /// Adds time to the timer.
    /// </summary>
    /// <param name="value">Amount of time to add</param>
    public void TimeAddition(float value)
    {
        timer.value += value;
    }

    /*List Stuff*/
    /// <summary>
    /// Adds an item to the list of items.
    /// </summary>
    /// <param name="gameObject">The game object to add</param>
    public void AddItemToList(GameObject gameObject)
    { 
        itemList.Add(gameObject);
    }

    /// <summary>
    /// Deletes an item's game object and removes it from list of items.
    /// </summary>
    /// <param name="gameObject">The Game Object to Remove</param>
    public void DeleteItem(GameObject gameObject)
    {
        itemList.Remove(gameObject);
        GameObject.Destroy(gameObject);
    }
    #endregion

}