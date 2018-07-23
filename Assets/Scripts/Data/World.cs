using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//This is the main World, this object is the main data object that helds and manages the other data and the game state
public class World{

#region Game Data Objects

    //List of the current levels (or plataforms) 
    public Level[] levels;
    //List of the current holes active in the level
    public List<Hole> holes;
    //List of enemies in the level
    public List<Enemy> enemies;

    //Main reference to the player object data
    public Player thePlayer;
    #endregion
    
#region Level measures

    //Size of the hole, this can be modified depending on the game desing
    public float holeWidth;
    //The hole speed, is the same as the player speed
    public float holeSpeed;

    //Total width of the game, it can be reflected on the level width
    public float width;
    //Total height of the level, it gives the manager a measure to calculate the distance between levels
    public float height;


    //The distance between levels, it depends on the game height
    private float heightDelta;
    public float HeightDelta { get { return heightDelta; } protected set { heightDelta = value; } }

    #endregion

#region Attributes

    //This variable manages the number of holes and enemies that are to appear on a game
    int currentDifficulty;
    public int CurrentDifficulty
    {
        get
        {
            return currentDifficulty;
        }

        protected set
        {
            currentDifficulty = value;
        }
    }

    //Number of current lives, 5 is the default
    int lives = 5;
    public int Lives
    {
        get
        {
            return lives;
        }

        protected set
        {
            lives = value;
        }
    }
   
    //If the player got out of the first level/plataform, it can lose lives if they return, so we must keep a flag on it
    bool gotOutOfFirstLevel;

    #endregion
    
#region Callbacks
    //This Action attributes are callbacks to subscribe to recive events on other components

    //When a level is created   
    public Action<Level> cbOnLevelCreated;
    //When a hole is created 
    public Action<Hole> cbOnHoleCreated;
    //When a hole is deleted, this is important to unsubscribe to events
    public Action<Hole> cbOnHoleDeleted;

    //When a new enemy is created
    public Action<Enemy> cbOnEnemyCreated;
    //When an enemy is deleted
    public Action<Enemy> cbOnEnemyDeleted;

    //This callback allows other objects in the data level to update in time
    public Action<float> cbWorldUpdate;

    //When the player is created
    public Action<Player> onPlayerCreated;
    #endregion

   /// <summary>
   /// Main Constructor of the world
   /// </summary>
   /// <param name="width"> The world width in units</param>
   /// <param name="height"> The world height in units</param>
   /// <param name="speed"> Speed of the player and the holes</param>
   /// <param name="floorsNumber"> Number of levels, + base level + last level</param>
    public World(float width, float height,float speed,int floorsNumber = 9)
    {
        this.width = width;
        this.height = height;
        currentDifficulty = 0;

        holeSpeed = speed;
        //This is an arbitrary value, it can be change if required
        holeWidth = width / 7;

        //Create the lists
        holes = new List<Hole>();
        enemies = new List<Enemy>();
        levels = new Level[floorsNumber];


        //Start to iterate and create the levels
        float currentHeight = 0;
        heightDelta = height / floorsNumber;

        for (int i = 0; i < floorsNumber; i++)
        {
            levels[i] = new Level(i, width, currentHeight);
            currentHeight += heightDelta;            
        }

        //Initiates the player
        thePlayer = new Player(this, speed, heightDelta/4f, 0.5f);

        //If there is a callback, call all the methods subscibed to it
        if (onPlayerCreated!= null)
        {
            onPlayerCreated(thePlayer);
        }

        //Start first level
        StartNextLevel();
    }

    /// <summary>
    /// This is the method that is called every update and informs all objects listening
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    public void WorldUpdate(float deltaTime)
    {
        if (cbWorldUpdate != null)
        {
            cbWorldUpdate(deltaTime);
        }
    }

    /// <summary>
    /// Start the next level
    /// </summary>
    void StartNextLevel()
    {
        //Clear holes and enemies
        Clear();
        //Set the player in the first level
        thePlayer.ResetPosition(levels[0]);

        //Ups the difficulty
        currentDifficulty++;

        //3 holes as base, and add one more with every level
        int holesNumber = 3 + currentDifficulty;
        for (int i = 0; i < holesNumber; i++)
        {
            Hole hole = new Hole(this, levels[UnityEngine.Random.Range(1, levels.Length-1)], holeWidth, UnityEngine.Random.Range(0,1)>0.5);
            holes.Add(hole);
            //Call the callback
            if(cbOnHoleCreated != null)
                cbOnHoleCreated(hole);
        }

        //Adds the enemies, the difficulty -1
        for (int i = 0; i < currentDifficulty-1 ; i++)
        {
            Enemy enemy = new Enemy(this, levels[UnityEngine.Random.Range(1, levels.Length - 1)], thePlayer.Height * 2);
            enemies.Add(enemy);
            //Call the callback
            if(cbOnEnemyCreated != null)
            {
                cbOnEnemyCreated(enemy);
            }
        }
    }

    /// <summary>
    /// Each time the player jumps, a new hole is created 
    /// </summary>
    public void AddHole()
    {
        Hole hole = new Hole(this, levels[UnityEngine.Random.Range(1, levels.Length - 1)], holeWidth, UnityEngine.Random.Range(0, 1) > 0.5);
        holes.Add(hole);
        cbOnHoleCreated(hole);
    }

    /// <summary>
    /// This function cleans the current enemies and the current holes
    /// </summary>
    void Clear()
    {
        for (int i = 0; i < holes.Count; i++)
        {
            if(cbOnHoleDeleted != null)
            {
                cbOnHoleDeleted(holes[i]);
            }
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (cbOnEnemyDeleted != null)
            {
                cbOnEnemyDeleted(enemies[i]);
            }
        }

        holes.Clear();
        enemies.Clear();
    }

    #region Get Level Functions
    /// <summary>
    /// Returns the next level based on the movement patron. Used by holes to loop around the level
    /// </summary>
    /// <param name="currentLevel">Where am I?</param>
    /// <param name="goingUp">Where am I going? Up = true, down = false</param>
    /// <returns>The next level if exist, null otherwise</returns>
    public Level GetNextLevelUpAndDown(int currentLevel, bool goingUp)
    {
        //Check borders to handle edge cases
        if (currentLevel + 1 == levels.Length && goingUp)
        {
            return null;
        } else if (currentLevel == 1 && !goingUp)
        {
            return null;
        }

        return (goingUp)? levels[currentLevel + 1]: levels[currentLevel - 1];
    }

    /// <summary>
    /// Gets the next level going upwards, Used by the player
    /// </summary>
    /// <param name="currentLevel">Where am I?</param>
    /// <returns>The next level if exist, null otherwise</returns>
    public Level GetNextLevel(int currentLevel = -1)
    { 
        //Activates the flag if the player leaves the first level
        if(currentLevel == 0)
        {
            gotOutOfFirstLevel = true;
        }

        if(currentLevel + 1 == levels.Length)
        {
            //You win! you reached the last level.
            //Start the next level from level 0
            StartNextLevel();
            return levels[0];
        }
        return levels[currentLevel + 1];
    }

    /// <summary>
    /// Gets the previous level, used by the player
    /// </summary>
    /// <param name="currentLevel">Where am I?</param>
    /// <returns>The previous level if exist, null otherwise</returns>
    public Level GetPreviousLevel(int currentLevel)
    {
        if (currentLevel == 1)
        {
            //IF we reached the bottom after leaving it, you lose a life
            if (gotOutOfFirstLevel)
            {
                lives--;
                if(lives == 0)
                {
                    //Resets the game 
                    currentDifficulty = 0;
                    StartNextLevel();
                }
            }
            //Restart the flag
            gotOutOfFirstLevel = false;
        }

        //Edgecases
        if (currentLevel == 0)
        {
            return null;
        }
        return levels[currentLevel - 1];
    }
    #endregion
    #region Register/Unregister callbacks methods
    public void RegisterOnPlayerCreated(Action<Player> onPlayerCreated)
    {
        this.onPlayerCreated += onPlayerCreated;
    }

    public void UnregisterOnPlayerCreated(Action<Player> onPlayerCreated)
    {
        this.onPlayerCreated -= onPlayerCreated;
    }

    public void RegisterOnEnemyCreated(Action<Enemy> onEnemyCreated)
    {
        this.cbOnEnemyCreated += onEnemyCreated;
    }
    public void UnregisterOnEnemyCreated(Action<Enemy> onEnemyCreated)
    {
        this.cbOnEnemyCreated += onEnemyCreated;
    }

    public void RegisterOnEnemyRemove(Action<Enemy> onEnemyRemoved)
    {
        this.cbOnEnemyDeleted += onEnemyRemoved;
    }
    public void UnregisterOnEnemyRemove(Action<Enemy> onEnemyRemoved)
    {
        this.cbOnEnemyDeleted -= onEnemyRemoved;
    }

    public void RegisterOnLevelCreated (Action<Level> onLevelCreated)
    {
        cbOnLevelCreated += onLevelCreated;
    }
    public void UnRegisterOnLevelCreated (Action<Level> onLevelCreated)
    {
        cbOnLevelCreated -= onLevelCreated;
    }
    public void RegisterOnHoleCreated(Action<Hole> onHoleCreated)
    {
        cbOnHoleCreated += onHoleCreated;
    }

    public void UnRegisterOnHoleCreated(Action<Hole> onLevelCreated)
    {
        cbOnHoleCreated -= onLevelCreated;

    }

    public void RegisterOnHoleDeleted(Action<Hole> onHoleDeleted)
    {
        cbOnHoleDeleted += onHoleDeleted;
    }

    public void UnregisterOnHoleDeleted(Action<Hole> onHoleDeleted)
    {
        cbOnHoleDeleted -= onHoleDeleted;
    }

    public void RegisterUpdateWorld(Action<float> onUpdateWorld)
    {
        cbWorldUpdate += onUpdateWorld;
    }

    public void UnRegisterUpdateWorld(Action<float> onUpdateWorld)
    {
        cbWorldUpdate += onUpdateWorld;
    }
    #endregion
}
