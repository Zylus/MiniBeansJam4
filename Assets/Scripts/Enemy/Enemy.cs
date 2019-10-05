using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    Patrol,
    Chase,
    FindGhost
}

public class Enemy
{
    public const float DEFAULT_SPEED = 5f;
    public const float DEFAULT_ROTATION_SPEED = 80f;

    public float Speed { get; set; }
    public float RotationSpeed { get; set; }

    public AIState ChaseStatus { get; set; }
    public Vector2 LastKnownPlayerPosition { get; set; }

    public void Init()
    {
        Speed = DEFAULT_SPEED;
        RotationSpeed = DEFAULT_ROTATION_SPEED;
    }

    public Vector2 GetRandomPatrolPoint()
    {
        return new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    public Vector2 GetGhostPoint()
    {
        float factor = Random.Range(1f, 5f);
        Vector2 playerMovementVector = GameController.Instance.Player.Model.MovementVector;
        return new Vector2(LastKnownPlayerPosition.x + playerMovementVector.x * factor, LastKnownPlayerPosition.y + playerMovementVector.y * factor);
    }

    public float GetActualMovement(float translation)
    {
        float actualTranslation = translation * Speed;
        actualTranslation *= Time.deltaTime;
        return actualTranslation;
    }

    public float GetActualRotation(float rotation)
    {
        float actualRotation = rotation * RotationSpeed * -1;
        actualRotation *= Time.deltaTime;
        return actualRotation;
    }
}
