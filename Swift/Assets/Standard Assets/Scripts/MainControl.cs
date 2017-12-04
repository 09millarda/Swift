using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MainControl : MonoBehaviour {

	public GameObject player;        // Player GameObject
	public TextMesh scoreText;      // Score text
	public GameObject circle;       // Circle Gameobject
	private int rotateSpeed = 100;      // Rotation speed of the player
	public bool isPlaying = true;      // Whether the game is playing
	private int score;      // Player score
	public List<GameObject> blocks = new List<GameObject>();        // A list of all the blocks
	public GameObject block;        // Block gameobject
	private int currBlock;      // The current block to jump
	public GameObject BLOCKS;   // Parent of all the blocks
	public int jumpThrust = 10;     // Jump force of the blocks
	public bool[] blockSpaces = new bool[10];    // Sections of the circle where blocks can spawn. whether they are taken up or not.
	public int spawnNumber = 0;         // number of blocks to spawn
	System.Random rnd = new System.Random();
	public GameObject[] thingsToHide;       // UI items to hide when playing the game
	public Button startButton;      // S.E.
	public Button githubButton;     // S.E.
    public Button continueButton;   // S.E.
	public bool onUI = true;        // if currently on gui not game
    private GameObject whiteBackground;     // white background that fades in
    public GameObject menu2;        // the menu when you lose
    public GameObject menu1;        // main menu

    // SOUNDS
    public AudioClip point;
    public AudioClip jump;
    public AudioClip gameOver;
    public AudioSource audioSource;

    // Use this for initialization
    void Start () {
		scoreText.text = "GO";
		isPlaying = false;
		setBlockSpacesToEmpty();

		startButton.onClick.AddListener(hideAll);
		githubButton.onClick.AddListener(openGit);
        continueButton.onClick.AddListener(goToMenuMain);
        whiteBackground = GameObject.Find("WhiteBackground");
	}

    void goToMenuMain()
    {
        menu2.SetActive(false);
        menu1.SetActive(true);
        destroyBlocks();
        player.transform.position = new Vector3(0, 3.035f, 0.063f);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        scoreText.text = "GO";
        currBlock = 0;
        spawnNumber = 0;
        score = 0;
    }
	
	public void openGit()
	{
		Application.OpenURL("https://github.com/09millarda");
	}

	// Update is called once per frame
	void Update () {
		// Rotate player around center if the game is playing
		if(isPlaying)
		{
			player.transform.RotateAround(Vector3.zero, Vector3.forward, -rotateSpeed * Time.deltaTime);

			if (Input.GetMouseButtonDown(0))
			{
				if (currBlock < blocks.Count)
				{
					blockJump(currBlock);
					currBlock++;
				}
			}
		} else
		{
			if (Input.GetMouseButtonDown(0) && !onUI)
			{ 
				string clickedObj = CastRay();

				// click control for not playing game
				switch(clickedObj)
				{
					case "Circle":
						startPlaying();
						break;
					default:
						break;
				}
			}
		}               
	}

	// hide all GOs in the ThingsToHide Array
	public void hideAll()
	{
		foreach(GameObject go in thingsToHide)
		{
			go.SetActive(false);
		}
		onUI = false;
	}

	// hide all GOs in the ThingsToHide Array
	public void showAll()
	{
		foreach (GameObject go in thingsToHide)
		{
			go.SetActive(true);
		}
	}

	// Start the game
	void startPlaying()
	{
		isPlaying = true;
		score = 0;
		spawnNumber = 1;
		scoreText.text = score.ToString();
		instatiateBlock(-1);
	}

	// When 1 rotation is completed
	public void fullCircle()
	{
		if(isPlaying)
		{
			destroyBlocks();
			score++;
			scoreText.text = score.ToString();
			currBlock = 0;
			setBlockSpacesToEmpty();
            audioSource.clip = point;
            audioSource.Play();
		}
		spawnBlocks();
	}

    // end the game
	public void endGame()
	{
        audioSource.clip = gameOver;
        audioSource.Play();
        onUI = true;
		isPlaying = false;
        foreach(GameObject go in blocks)
        {
            var physics = go.GetComponent<Rigidbody2D>();
            physics.simulated = false;
        }

        whiteBackground.SetActive(true);
        Color tmp = whiteBackground.GetComponent<SpriteRenderer>().color;
        tmp.a = 0f;
        whiteBackground.GetComponent<SpriteRenderer>().color = tmp;

        StartCoroutine(whiteBackground.GetComponent<WhiteBackgroundFadeIn>().FadeIn(whiteBackground.GetComponent<SpriteRenderer>()));
    }

    // show and update scores
    public void showMenu2()
    {
        menu2.SetActive(true);
        GameObject.Find("yourScore").GetComponent<Text>().text = "You Scored: " + score.ToString();

        if(PlayerPrefs.HasKey("HighScore"))
        {
            int highScore = PlayerPrefs.GetInt("HighScore");

            if(score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", score);
                highScore = score;
            }
            GameObject.Find("highScore").GetComponent<Text>().text = "High Score: " + highScore.ToString();
        } else
        {
            PlayerPrefs.SetInt("HighScore", score);
            GameObject.Find("highScore").GetComponent<Text>().text = "High Score: " + score.ToString();
        }
        PlayerPrefs.Save();
    }

	// Makes the selected box jump
	void blockJump(int index)
	{
		Vector2 blockPos = blocks[index].transform.position;

		blocks[index].GetComponent<Rigidbody2D>().AddForce(blockPos * jumpThrust);

        audioSource.clip = jump;
        audioSource.Play();
    }

	// reset block spaces to false
	void setBlockSpacesToEmpty()
	{
		for(int i = 0; i < blockSpaces.Length; i++)
		{
			blockSpaces[i] = false;
		}
	}

	// Spawn the blocks
	void spawnBlocks()
	{
		// Every 5th rotation, add 1 more block to the circle. Max is 10.
		if(score % 5 == 0)
		{
			if(spawnNumber < 10)
			{
				spawnNumber++;      // Only increase if spawnNumber is less than 10 (10 is the max number allowed)
			}			
		}

		// Generate the random positions
		for(var i = 0; i < spawnNumber; i++)
		{
			int spawnLocation;
			// check if blockSpaces is available
			while(true)
			{
				spawnLocation = rnd.Next(0, 9);     // Select one of the random spawn locations
				if(!blockSpaces[spawnLocation])
				{
					blockSpaces[spawnLocation] = true;
					break;
				}
			}           
		}
		if (isPlaying)
		{
			// Add blocks to the blocks List of gameobjects in order of rotational position from 0 degrees
			for(int i = 0; i < blockSpaces.Length; i++)
			{
				if(blockSpaces[i] == true)
				{
					instatiateBlock(i);     // instantiate
				}
			}            
		}
	}

	void destroyBlocks()
	{
		foreach (GameObject go in blocks)
			Destroy(go);    // delete all the block gameobjects

		blocks = new List<GameObject>();        // empty the list of blocks
	}

	// Instantiate Block
	void instatiateBlock(int spawnLocation)
	{
		int angleOfPlacement = 0;       // The angle to place the block in degrees

		// if spawnLocation == -1, first round so put at bottom of circle.
		if(spawnLocation == -1)
		{
			angleOfPlacement = 180;
		} else
		{
			angleOfPlacement = 55 + (spawnLocation * 25);        // 45 degrees either side blocks cant spawn. this is how to calculate the spawn angle.
		}

		Vector3 spawnPosition = new Vector3(0, 3.47f, -1);      // Vector 3 that represents the position to spawn the block (top of circle)
		Vector3 rotationVector = new Vector3(0, 0, angleOfPlacement);       // Vector 3 of rotation to apply to the block
		var dir = spawnPosition - (new Vector3(0, 0, 0));   // gets the block relative to the pivot
		dir = Quaternion.Euler(-rotationVector) * dir;       // Rotate the point "-" rotationVector due to Matrix laws. (Always acts the wrong way to what you think...)
															 // It's why i did rubbish in complex maths...
		spawnPosition = dir + new Vector3(0, 0, -1);     // Calculate the rotated point

		GameObject newBlock = Instantiate(block, spawnPosition, Quaternion.Euler(0f, 0f, -angleOfPlacement));       // instantiate block
		newBlock.transform.parent = BLOCKS.transform;       // set parent of block to Blocks Game Object

		blocks.Add(newBlock);
	}

	// Cast ray for click detection
	string CastRay()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);
		if(hit)
		{
			return hit.collider.gameObject.tag;
		}
		return "";
	}
}
