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
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject Hint;

    [Header("Audio")]
    [SerializeField] AudioSource Title_Theme;
    [SerializeField] AudioSource Main_Theme;
    [SerializeField] AudioSource Win_Theme;
    [SerializeField] AudioSource Loss_Theme;
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

    private State state = State.TITLE;
    // Start is called before the first frame update
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
                if (Input.GetKey(KeyCode.Tab))
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
                CheckRunes();
                //temp
                if (Input.GetKeyDown(KeyCode.O)) state = State.GAME_WIN;
                if (Input.GetKeyDown(KeyCode.L)) state = State.GAME_OVER;

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
                Lose.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                break;
            default:
                break;
        }
        UIManager.Instance.Timer = timer;
    }

    public void OnStartGame()
    {
        state = State.START_GAME;
        print("Start_game");
    }

    public void BackToTitle()
    {
        ResetRunes();
        if (Win.activeSelf == true)
        { 
            Win.SetActive(false);
            Win_Theme.Stop();
        }
        if(Lose.activeSelf == true)
        {
            Lose.SetActive(false);
            Loss_Theme.Stop();
        }
        state = State.TITLE;
    }


    public void OnPlayerDead()
    {
        //state = State.START_GAME;
    }

    public void OnAddPoints(int points)
    {
        print(points);
        score.value += points;
    }

    //RUNES STUFF
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
    }
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

    public void OnPlayGame()
    {
        print("Play Game");
        state = State.PLAY_GAME;
    }
}