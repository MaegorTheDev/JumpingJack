using UnityEngine;
using System;

public class Enemy
{
    #region Attributes

    /// <summary>
    /// World reference
    /// </summary>
    World world;
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

    /// <summary>
    /// My current level
    /// </summary>
    private Level currentLevel;
    public Level CurrentLevel { get { return currentLevel; } protected set { currentLevel = value; } }

    /// <summary>
    /// Width/height of the enemy hitbox
    /// </summary>
    private float width;
    public float Width { get { return width; } protected set { width = value; } }

    /// <summary>
    /// Current y position, calculated with the current level and my width/height
    /// </summary>
    private float y;
    public float Y
    {
        get
        {
            y = currentLevel.Y + Width/2;
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
    public float X
    {
        get { return x; }
        protected set { x = value; }
    }

    #endregion

    /// <summary>
    /// My Movement speed
    /// </summary>
    float speed;

    /// <summary>
    /// Movement callback
    /// </summary>
    public Action<Enemy> cbEnemyChange;


    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="world">World reference</param>
    /// <param name="currentLevel">The initial level</param>
    /// <param name="width">How T H I C C is the enemy</param>
    public Enemy(World world, Level currentLevel, float width)
    {
        this.world = world;
        this.currentLevel = currentLevel;
        this.width = width;

        //Randomize speed and X position
        X = UnityEngine.Random.Range(-currentLevel.Width / 2, currentLevel.Width / 2);
        speed = UnityEngine.Random.Range(World.holeSpeed/2,World.holeSpeed);
        if (UnityEngine.Random.Range(0, 1) > 0.5f)
        {
            speed *= -1;
        }

        world.RegisterUpdateWorld(EnemyUpdate);
    }

    /// <summary>
    /// Update Method
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    public void EnemyUpdate(float deltaTime)
    {
        //It loops around the level
        x += speed * deltaTime;
        if (x > currentLevel.Width / 2)
        {
            x = -currentLevel.Width / 2;
        }
        else if (x < -currentLevel.Width / 2)
        {
            x = currentLevel.Width / 2;
        }

        if (cbEnemyChange != null)
        {
            cbEnemyChange(this);
        }
    }

    #region Callback Register/unregister
    public void RegisterEnemyChange(Action<Enemy> OnEnemyChange)
    {
        cbEnemyChange += OnEnemyChange;
    }

    public void UnregisterEnemyChange(Action<Enemy> OnEnemyChange)
    {
        cbEnemyChange -= OnEnemyChange;
    }

    #endregion
}
