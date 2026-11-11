using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerController
{
    float NowMoveSpeed { get; set; }

    event Action<bool> OnGroundChanged;
    bool IsGround { get; }

    Vector2 MoveInput { get; set; }
    void SetMoveInput(Vector2 moveInput);
    float MoveInputUp { get; set; }
    float MoveInputDown { get; set; }

    void SyncCamera();
    void Jump();

    void FixedUpdate() { }
    void Update() { }
    void LateUpdate() { }
}


public interface IPlayerPhysicsController
{
    float NowMoveSpeed { get; set; }
    event Action<bool> OnGroundChanged;
    bool IsGround { get; } // персонаж на земле (работает во всех средах)
    Vector3 MoveInput { get; set; } // если персонаж ходит по земле то учитывается только двумерная составляющая
    void SyncCamera(); // камера синхронизируется с направлением движения персонажа
    void Jump(); // прыжок
    void FixedUpdate() { }
    void Update() { }
    void LateUpdate() { }
}


// public interface IPlayerMoveSpeed
// {
//     float GetNowSpeed();
// }


// public interface IPlayerRestrictions
// {

// }

// public struct EnvironmentContext
// {
//     public bool Ground;
//     public bool Wall;
//     public bool Air;

//     public bool Water;
//     public bool WaterSurface;

//     public bool Ladder;
//     public bool Cable;
// }


// public struct EnvironmentContext
// {
//     public bool HasGround;
//     public float GroundAngle;

//     public bool HasWall;
//     public float WallAngle;

//     public bool InWater;
//     public float WaterDepth;

//     public bool OnLadder;
//     public bool OnCable;
// }

// public class EnvironmentDetector
// {
//     private IPlayerPhysicsBody OldPlayerPhysicsBody;
//     public event Action<EnvironmentContext> OnEnvironmentChanged;
//     public event Action<PlayerPhysicsBody> OnPlayerPhysicsBodyChanged;
//     public void FixedUpdate()
//     {
//         // определяем среду и предпочтительное для этой среды тело IPlayerPhysicsBody
//     }
// }


