using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data object of a level
/// </summary>
public class Level {

    /// <summary>
    /// The level number of the level
    /// </summary>
    private int levelNumber;
    public int LevelNumber { get { return levelNumber; } protected set { levelNumber = value; } }

    /// <summary>
    /// Width in units
    /// </summary>
    private float width;
    public float Width { get { return width; } protected set { width = value; } }

    /// <summary>
    /// Y position of the level
    /// </summary>
    private float y;
    public float Y { get { return y; } protected set { y = value; } }


    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="levelNumber">Level number</param>
    /// <param name="width">Width</param>
    /// <param name="y">Y position</param>
    public Level(int levelNumber, float width, float y)
    {
        this.levelNumber = levelNumber;
        this.width = width;
        this.y = y;

    }
}
