using UnityEngine;
using System;

/// <summary>
/// Struct to pass the collisions from the collision handler controller
/// </summary>
public struct CollisionInfo
{
    public bool above, below;

    public void Reset()
    {
        above = below = false;
    }
}
/// <summary>
/// The player data object, it handles movement, jumps and states. It does not handle collisions
/// </summary>
public class Player  {
    
    /// <summary>
    /// Collision object 
    /// </summary>
    public CollisionInfo collisionLevels = new CollisionInfo();

    #region Attributes

    /// <summary>
    /// The x position of the player
    /// </summary>
    private float x;
    public float X
    {
        get
        {
            return x;
        }

        protected set
        {
            //In the set, we update the callback if it exist
            if (cbOnCharacterMove != null)
                cbOnCharacterMove(x, y);
            x = value;
        }
    }

    /// <summary>
    /// The y position of the player
    /// </summary>
    private float y;
    public float Y
    {
        get
        {
            return y;
        }

        protected set
        {
            //In the set, we update the callback if it exist
            if (cbOnCharacterMove != null)
                cbOnCharacterMove(x,y);
            y = value;
        }
    }

    /// <summary>
    /// Height of the player, used to calculate collisions and level state
    /// </summary>
    private float height;
    public float Height
    {
        get
        {
            return height;
        }

        protected set
        {
            height = value;
        }
    }

    /// <summary>
    /// Movement speed
    /// </summary>
    private float speed;
    public float Speed
    {
        get
        {
            return speed;
        }

        protected set
        {
            speed = value;
        }
    }

    /// <summary>
    /// How high does the player goes when it jumps
    /// </summary>
    float jumpHeight;
    public float JumpHeight
    {
        get
        {
            return jumpHeight;
        }

        protected set
        {
            jumpHeight = value;
        }
    }
    
    /// <summary>
    /// Time in seconds for the player to reach the apex of the jumps
    /// Instead of using gravity, this variable is easier to understand.
    /// </summary>
    float timeToJumpApex = .4f;
    public float TimeToJumpApex
    {
        get
        {
            return timeToJumpApex;
        }

        set
        {
            timeToJumpApex = value;
        }
    }

    /// <summary>
    /// Velocity vector of the player
    /// </summary>
    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }

        protected set
        {
            velocity = value;
        }
    }

    /// <summary>
    /// Is the player stunned?
    /// </summary>
    bool stunned;
    public bool Stunned
    {
        get
        {
            return stunned;
        }

        set
        {
            stunned = value;
            //Update the callback
            if (cbOnPlayerStunned != null)
            {
                cbOnPlayerStunned();
            }
        }
    }

    #endregion

    /// <summary>
    /// Gravity of the jump, this is calculated based on jump height and jump time to apex
    /// </summary>
    float gravity;
    /// <summary>
    /// Initial velocity of the jump. Calculated based on jump height and jump time to apex
    /// </summary>
    float jumpVelocity;
    /// <summary>
    /// My current level
    /// </summary>
    Level currentLevel;

    /// <summary>
    /// Reference to the world object
    /// </summary>
    World world;

    /// <summary>
    /// This bool manages if the player is in contact with a hole above, if it is when jumping it ignores the collision with the floor
    /// </summary>
    bool isTouchingHoleAbove;
    /// <summary>
    /// This bool manages if the player is in contact with a hole below, if it is gravity is apply as if there was no level below
    /// </summary>
    bool isTouchingHoleBelow;

    /// <summary>
    /// Speed input normalized for x axis 
    /// </summary>
    float normalizedXMovement;
    /// <summary>
    /// Input manager that tells the player data object that it must jump this frame
    /// </summary>
    bool isJumpingThisFrame;

    /// <summary>
    /// Time stunned in seconds
    /// </summary>
    float TotalStunnedTime = 2f;
    /// <summary>
    /// how much time have I been stuned
    /// </summary>
    float CurrentSunnedTime;

    #region Callbacks
    /// <summary>
    /// Callback on character moving
    /// </summary>
    public Action<float,float> cbOnCharacterMove;
    /// <summary>
    /// Callback on character stunned
    /// </summary>
    public Action cbOnPlayerStunned;

    #endregion


    /// <summary>
    /// Main cosntructor
    /// </summary>
    /// <param name="world">Reference to the world object</param>
    /// <param name="speed">Movement speed</param>
    /// <param name="height">My height</param>
    /// <param name="timeToJumpApex"> How much time do I need to reach the apex of a jump</param>
    public Player(World world, float speed, float height, float timeToJumpApex)
    {
        this.world = world;
        this.speed = speed;
        this.height = height;
        this.timeToJumpApex = timeToJumpApex;

        //The jump height depends of the delta between levels
        jumpHeight = world.HeightDelta * 1.5f;
        
        //Calculate jump variabñes
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //Gets the first level
        currentLevel = world.GetNextLevel();

        //Register to the update method
        world.RegisterUpdateWorld(MoveUpdate);
    }

    /// <summary>
    /// Method that gets call every iteration of update
    /// </summary>
    /// <param name="timeDelta">Time.deltaTime</param>
    public void MoveUpdate(float timeDelta)
    {
        //If I am stunned keep counting the countdown
        if (Stunned)
        {
            StunnedCountdown(timeDelta);
            
        }

        //Check my current level based on my y position
        CheckCurrentLevel();
        //Process the collisions informed by the collision component
        ProcessCollisions(timeDelta);
        //Move manager
        Move(timeDelta);
    }

    /// <summary>
    /// It takes the current state of collisions and handle it accordinly
    /// </summary>
    /// <param name="timeDelta"></param>
    void ProcessCollisions(float timeDelta)
    {
        //If I hit the bottom part of a level I get stunned, and no more
        if (collisionLevels.above && !Stunned)
        {
            Stunned = true;
            velocity.y = 0;
            return;
        }

        //If I am touching a floor below me and there is no hole, keep my y velocity in 0
        if (collisionLevels.below && !isTouchingHoleBelow)
        {
            velocity.x = normalizedXMovement * speed;
            velocity.y = 0;
            y = currentLevel.Y;
        }
        //If I am touching the hole below, apply gravity
        else if (isTouchingHoleBelow)
        {
            velocity.x = 0;
            velocity.y += (gravity) * timeDelta;
            //collisionLevels.below = false;
        }

        //Jump! if the player should jump this frame and is grounded
        if (isJumpingThisFrame && collisionLevels.below)
        {
            velocity.y += jumpVelocity;
            collisionLevels.below = false;
            isJumpingThisFrame = false;
            
            //No moving in x while jumping
            velocity.x = 0;
        }
        
        //Apply gravity
        velocity.y += (collisionLevels.below) ? 0 : gravity * timeDelta;
    }


    /// <summary>
    /// Apply movement
    /// </summary>
    /// <param name="timeDelta"></param>
    void Move(float timeDelta)
    {
        //MoveHorizontal. It loops around the level when it reaches the limit
        x += velocity.x * timeDelta;
        if (x > currentLevel.Width / 2)
        {
            x = -currentLevel.Width / 2;
        }
        else if (x < -currentLevel.Width / 2)
        {
            x = currentLevel.Width / 2;
        }


        //MoveVertical.
        y += velocity.y * timeDelta;

        //Callback
        if (cbOnCharacterMove != null)
        {
            cbOnCharacterMove(x, y);
        }
    }

    /// <summary>
    /// This method is called when you hit an enemy. Stuns you and resets your velocity and movmente
    /// </summary>
    public void Stun()
    {
        Stunned = true;
        velocity = Vector2.zero;
        normalizedXMovement = 0;
    }

    /// <summary>
    /// Method called by the manager component, this handles the collisions of the level
    /// </summary>
    /// <param name="collisionInfo"></param>
    public void HandleLevelCollision(CollisionInfo collisionInfo)
    {        
        collisionLevels.above = (isTouchingHoleAbove)? false: collisionInfo.above;
        collisionLevels.below = (isTouchingHoleBelow) ? false : collisionInfo.below;
    }

    /// <summary>
    /// Countdown to de-stun the player
    /// </summary>
    /// <param name="timeDelta"></param>
    void StunnedCountdown(float timeDelta)
    {
        CurrentSunnedTime += timeDelta;

        if(CurrentSunnedTime >= TotalStunnedTime)
        {
            Stunned = false;
            CurrentSunnedTime = 0;
        }
    }

    /// <summary>
    /// It changes the state of the bool attribute if its touching a hole below
    /// </summary>
    /// <param name="state"> new state</param>
    public void ChangeTouchHoleBelow(bool state)
    {
        isTouchingHoleBelow = state;
    }
    /// <summary>
    /// It changes the state of the bool attribute if its touching a hole above
    /// </summary>
    /// <param name="state">new state</param>
    public void ChangeTouchHoleAbove(bool state)
    {
        isTouchingHoleAbove = state;
    }

    /// <summary>
    /// Checks in which level am I based on my Y position
    /// </summary>
    void CheckCurrentLevel()
    {
        
        if(y > currentLevel.Y + world.HeightDelta)
        {
            currentLevel = world.GetNextLevel(currentLevel.LevelNumber);
        }     
        //If my top part is below my current level
        if(y + height < currentLevel.Y && currentLevel.LevelNumber != 0)
        {
            currentLevel = world.GetPreviousLevel(currentLevel.LevelNumber);
        }
        
    }

    /// <summary>
    /// Resets the position of the player
    /// </summary>
    /// <param name="firstLevel">First level</param>
    public void ResetPosition(Level firstLevel)
    {
        this.x = 0;
        this.y = firstLevel.Y;
        currentLevel = firstLevel;

        if(cbOnCharacterMove != null)
            cbOnCharacterMove(x, y);

        velocity = Vector2.zero;
        Stunned = false;
        CurrentSunnedTime = 0;
    }

    /// <summary>
    /// Input manager handles this method
    /// </summary>
    /// <param name="input"></param>
    public void UpdateXMovement(float input)
    {
        normalizedXMovement = input;
    }

    /// <summary>
    /// Jump action"
    /// </summary>
    public void Jump()
    {
        isJumpingThisFrame = true;
        world.AddHole();
    }

    #region Register/Unregister callback Methods
    public void RegisterOnCharacterMove(Action<float, float> cbActionMove)
    {
        cbOnCharacterMove += cbActionMove;
    }

    public void UnRegisterOnCharacterMove(Action<float, float> cbActionMove)
    {
        cbOnCharacterMove -= cbActionMove;
    }

    public void RegisterOnPlayerStunned(Action cbStunnedMove)
    {
        cbOnPlayerStunned += cbStunnedMove;
    }

    public void UnregisterOnPlayerStunned(Action cbStunnedMove)
    {
        cbOnPlayerStunned -= cbStunnedMove;
    }
    #endregion
}
