

using UnityEngine;
using System;

/// <summary>
/// Hole data object
/// </summary>
public class Hole {

    #region Attributes

    /// <summary>
    /// My current level
    /// </summary>
    private Level currentLevel;
    public Level CurrentLevel{ get{return currentLevel;} protected set {currentLevel = value;}}

    /// <summary>
    /// Width of the hole
    /// </summary>
    private float width;
    public float Width { get { return width; } protected set { width = value; } }

    /// <summary>
    /// Current Y position of the hole. It is calculated based on the level
    /// </summary>
    private float y;
    public float Y {
        get
        {
            y = currentLevel.Y;
            return y;
        }
        protected set
        {
            y = value;
        }
    }

    /// <summary>
    /// Current x position
    /// </summary>
    private float x;
    public float X { get { return x; }
        protected set { x = value; }
    }

    /// <summary>
    /// World reference
    /// </summary> 
    private World world;
    public World World
    {
        get
        {
            return world;
        }

        set
        {
            world = value;
        }
    }

    #endregion

    /// <summary>
    /// Speed of the hole
    /// </summary>
    float speed;
    /// <summary>
    /// Which way is the hole going. It changes when it reaches the bottom/top
    /// </summary>
    bool goingUp;

    /// <summary>
    /// Callback to follow the hole movement 
    /// </summary>
    public Action<Hole> cbHoleChange;

    /// <summary>
    /// Main constructor 
    /// </summary>
    /// <param name="world">World reference</param>
    /// <param name="currentLevel">Starting level</param>
    /// <param name="width">Width of the hole, decided by the world</param>
    /// <param name="goingUp">Which way do I go frist</param>
    public Hole(World world, Level currentLevel, float width, bool goingUp = false)
    {
        this.world = world;
        this.currentLevel = currentLevel;
        this.width = width;

        this.goingUp = goingUp;
        //X position is random inside the current level
        X = UnityEngine.Random.Range(-currentLevel.Width / 2, currentLevel.Width / 2);
        speed = World.holeSpeed;

        //Register the callback to get the update time
        world.RegisterUpdateWorld(HoleUpdate);
    }

    /// <summary>
    /// Method to be called each update loop
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    public void HoleUpdate(float deltaTime)
    { 
        //If I am moving up
        if (goingUp)
        {
            x = x - speed * deltaTime;
            if (x < -CurrentLevel.Width / 2 - width / 2)
            {
                //I reached the horizontal limit of the level. 
                Level nextLevel = world.GetNextLevelUpAndDown(currentLevel.LevelNumber, goingUp);

                //If there is no next level, change your direction and get the next level
                if (nextLevel == null)
                {
                    goingUp = false;
                    nextLevel = world.GetNextLevelUpAndDown(currentLevel.LevelNumber, goingUp);
                    return;
                }

                //Continue with the movement
                currentLevel = nextLevel;
                x = currentLevel.Width/2 + width / 2;
            }
        }
        else
        {
            x = x + speed * deltaTime;
            if (x > CurrentLevel.Width / 2 + width / 2)
            {

                //I reached the horizontal limit of the level. 
                Level nextLevel = world.GetNextLevelUpAndDown(currentLevel.LevelNumber, goingUp);

                if (nextLevel == null)
                {

                    //If there is no next level, change your direction and get the next level
                    goingUp = true;
                    nextLevel = world.GetNextLevelUpAndDown(currentLevel.LevelNumber, goingUp);
                    return;
                }

                currentLevel = nextLevel;
                x = -currentLevel.Width/2 - width / 2;
            }
        }

        //Update The callback
        if (cbHoleChange != null)
        {
            cbHoleChange(this);
        }
    }


}
