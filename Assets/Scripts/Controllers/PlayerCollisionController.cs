using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour {

    World world;
    GameObject playerGameObject;

    const float skinWidth = 0.015f;

    Player thePlayer;
    
    public int verticalRayCount = 5;
    float verticalRaySpacing;



    public int horizontalRayCount = 5;
    float horizontalRaySpacing;

    public LayerMask collisionLevelsMask;
    public LayerMask collisionHoleMask;
    public LayerMask collisionEnemyMask;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;

    CollisionInfo collisions;


    // Use this for initialization
    void Start()
    {
        world = GameManager.Instance.World;
        thePlayer = world.thePlayer;
    }

    private void Update()
    {
        if (playerGameObject == null)
        {
            playerGameObject = GetComponent<PlayerSpriteController>().GetPlayerGameObject();
            boxCollider = playerGameObject.GetComponentInChildren<BoxCollider2D>();

            CalculateRaySpacing();
        }

        UpdateRaycastOrigins();
        collisions.Reset();
        CheckHoleCollisions();
        CheckFloorCollisions();
        CheckEnemyCollisions();
    }

    void CheckHoleCollisions()
    {
        float rayLenght = thePlayer.Height/2 + skinWidth;
        float count = 0;

        if(thePlayer.Velocity.y > 0 )
        {
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOriginTop = raycastOrigins.topLeft;
                rayOriginTop += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOriginTop, Vector2.up, rayLenght, collisionHoleMask);
                if (hit)
                {
                    count++;
                }

            }
            thePlayer.ChangeTouchHoleAbove(count > verticalRayCount / 2);
        }

        else
        {

            rayLenght = thePlayer.Height;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOriginBottom = raycastOrigins.topLeft;
                rayOriginBottom += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOriginBottom, Vector2.down, rayLenght, collisionHoleMask);

                if (hit)
                {
                    count++;
                }

            }
            
            thePlayer.ChangeTouchHoleBelow(count > verticalRayCount / 2);
        }       
        
    }

    void CheckFloorCollisions()
    {
            float directionY = Mathf.Sign(thePlayer.Velocity.y);
            float rayLenght = thePlayer.Height/4;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottonLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght , collisionLevelsMask);
                if (hit)
                {
                float distance = (directionY > 0) ? 0f : 10f;

                   if (hit.distance <= distance)
                   {
                      collisions.below = directionY == -1;
                      collisions.above = directionY == 1;
                      thePlayer.HandleLevelCollision(collisions);
                   }
                }

            }
        
    }

    void CheckEnemyCollisions()
    {
        float rayLenght = thePlayer.Height / 4;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOriginLeft = raycastOrigins.bottonLeft;
            Vector2 rayOriginRight = raycastOrigins.bottomRight;

            rayOriginLeft += Vector2.up * (horizontalRayCount * i);
            rayOriginRight += Vector2.up * (horizontalRayCount * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOriginLeft, Vector2.left, rayLenght, collisionEnemyMask);

            RaycastHit2D hit2 = Physics2D.Raycast(rayOriginRight, Vector2.right, rayLenght, collisionEnemyMask);
            if (hit || hit2 )
            {
                if (hit.distance <= 0.5f || hit2.distance <= 0.5f)
                {
                    thePlayer.Stun();
                }
            }

        }

    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;

        raycastOrigins.bottonLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }


    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        horizontalRaySpacing = bounds.size.y / (verticalRayCount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottonLeft, bottomRight;
    }
}
