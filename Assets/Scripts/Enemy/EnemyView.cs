using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MessageSendingEventArgs : EventArgs
{
    public MessageType type { get; set; }
}

public class EnemyView : MonoBehaviour
{
    private const float MAX_SPEED_PATROL = 1f;
    private const float MAX_SPEED_GHOST = 3f;
    private const float MAX_SPEED_CHASE = 7f;
    private const float MAX_ROTATION_SPEED_PATROL = 300f;
    private const float MAX_ROTATION_SPEED_GHOST = 330f;
    private const float MAX_ROTATION_SPEED_CHASE = 480f;
    private const float MAX_ACCELERATION_PATROL = 6f;
    private const float MAX_ACCELERATION_GHOST = 8f;
    private const float MAX_ACCELERATION_CHASE = 10f;
    private const float SLOWDOWN_DISTANCE_PATROL = 0.6f;
    private const float SLOWDOWN_DISTANCE_GHOST = 1f;
    private const float SLOWDOWN_DISTANCE_CHASE = 0.4f;
    private const float SLOWDOWN_DISTANCE_APPROACH = 5f;
    private const float STOP_DISTANCE_APPROACH = 1f;
    
    private const float HEAT_FACTOR = 5f;
    private const float LERP_FACTOR = 0.8f;
    private const float SPOTTING_DISTANCE = 5f;
    private const float PLAYER_STOPPING_TIME = 5f;
    private const float PLAYER_CLEAR_MEMORY_TIME = 8f;
    private const float PLAYER_OFFENSE_MEMORY_TIME = 30f;

    private Enemy _model;
    [SerializeField] private GameObject _enemyShip;
    [SerializeField] private AIDestinationSetter _destinationSetter;
    [SerializeField] private AIPath _aiPath;

    private float _timeUntilPlayerMustBeStopped;

    public event EventHandler<MessageSendingEventArgs> MessageSendingEvent;

    public void Init(Enemy model)
    {
        _model = model;
        SetPatrolMode();
    }
    
    public void SetPatrolMode()
    {
        if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
        {
            Destroy(_destinationSetter.target.gameObject);
        }

        GameObject emptyGO = new GameObject("Waypoint");
        emptyGO.transform.position = _model.GetRandomPatrolPoint();
        _destinationSetter.target = emptyGO.transform;
        Debug.Log("SETTING PATROL MODE with point " + emptyGO.transform.position.ToString());
        _aiPath.maxSpeed = MAX_SPEED_PATROL;
        _aiPath.rotationSpeed = MAX_ROTATION_SPEED_PATROL;
        _aiPath.slowdownDistance = SLOWDOWN_DISTANCE_PATROL;
        _aiPath.maxAcceleration = MAX_ACCELERATION_PATROL;
        _model.ChaseStatus = AIState.Patrol;
        GameController.Instance.Player.Model.TimeUntilOffenseIsForgotten = 0f;
    }

    public void SetPlayerApproachMode()
    {
        if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
        {
            Destroy(_destinationSetter.target.gameObject);
        }
        _destinationSetter.target = GameController.Instance.Player.View.transform;
        Debug.Log("SETTING APPROACH MODE");
        _aiPath.maxSpeed = MAX_SPEED_PATROL;
        _aiPath.rotationSpeed = MAX_ROTATION_SPEED_PATROL;
        _aiPath.slowdownDistance = SLOWDOWN_DISTANCE_APPROACH;
        _aiPath.maxAcceleration = MAX_ACCELERATION_PATROL;
        _model.ChaseStatus = AIState.Approach;
        _timeUntilPlayerMustBeStopped = PLAYER_STOPPING_TIME;
        GameController.Instance.Player.Model.PlayerIsBeingChecked = true;
        EventHandler<MessageSendingEventArgs> handler = MessageSendingEvent;
        if (handler != null)
        {
            handler(this, new MessageSendingEventArgs()
            {
                type = MessageType.Halt
            });
        }
    }

    public void SetPlayerChaseMode()
    {
        if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
        {
            Destroy(_destinationSetter.target.gameObject);
        }
        
        GameController.Instance.Player.Model.TimeUntilOffenseIsForgotten = PLAYER_OFFENSE_MEMORY_TIME;
        _destinationSetter.target = GameController.Instance.Player.View.transform;
        _aiPath.maxSpeed = MAX_SPEED_CHASE;
        _aiPath.rotationSpeed = MAX_ROTATION_SPEED_CHASE;
        _aiPath.slowdownDistance = SLOWDOWN_DISTANCE_CHASE;
        _aiPath.maxAcceleration = MAX_ACCELERATION_CHASE;
        _model.ChaseStatus = AIState.Chase;
        EventHandler<MessageSendingEventArgs> handler = MessageSendingEvent;
        if (handler != null)
        {
            handler(this, new MessageSendingEventArgs()
            {
                type = MessageType.Hostile
            });
        }
    }

    public void SetFindGhostMode()
    {
        if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
        {
            Destroy(_destinationSetter.target.gameObject);
        }
        
        _model.LastKnownPlayerPosition = GameController.Instance.Player.View.transform.position;
        Quaternion rot = GameController.Instance.Player.View.transform.rotation;
        GameObject ghostGO = GameObject.Instantiate(GameController.Instance.PlayerGhostPrefab, _model.GetGhostPoint(), rot);
        ghostGO.name = "Waypoint";
        _destinationSetter.target = ghostGO.transform;
        Debug.Log("SETTING GHOST MODE with point " + ghostGO.transform.position.ToString());
        _aiPath.maxSpeed = MAX_SPEED_GHOST;
        _aiPath.rotationSpeed = MAX_ROTATION_SPEED_GHOST;
        _aiPath.slowdownDistance = SLOWDOWN_DISTANCE_GHOST;
        _aiPath.maxAcceleration = MAX_ACCELERATION_GHOST;
        _model.ChaseStatus = AIState.FindGhost;
    }

    void FixedUpdate()
    {
        if (_model.ChaseStatus == AIState.Chase && !PlayerIsStillCleared() && Vector2.Distance(transform.position, _destinationSetter.target.transform.position) <= 0.5f)
        {
            // reached player
            EventHandler<MessageSendingEventArgs> handler = MessageSendingEvent;
            if (handler != null)
            {
                handler(this, new MessageSendingEventArgs()
                {
                    type = MessageType.Fine
                });
            }
            GameController.Instance.PunishPlayer();
            GameController.Instance.Player.Model.TimeUntilClearIsForgotten = PLAYER_CLEAR_MEMORY_TIME;
            GameController.Instance.Player.Model.PlayerIsBeingChecked = false;
            SetPatrolMode();
        }

        // Refresh patrol path
        if (_aiPath.reachedEndOfPath && !_aiPath.pathPending)
        {
            if (_model.ChaseStatus == AIState.Patrol)
            {
                if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
                {
                    Destroy(_destinationSetter.target.gameObject);
                }

                GameObject emptyGO = new GameObject("Waypoint");
                emptyGO.transform.position = _model.GetRandomPatrolPoint();
                _destinationSetter.target = emptyGO.transform;
            }
            else if (_model.ChaseStatus == AIState.FindGhost)
            {
                EventHandler<MessageSendingEventArgs> handler = MessageSendingEvent;
                if (handler != null)
                {
                    handler(this, new MessageSendingEventArgs()
                    {
                        type = MessageType.Escaped
                    });
                }
                SetPatrolMode();
            }
        }

        if(_model.ChaseStatus == AIState.Approach)
        {
            CheckIfPlayerStopped();
        }

        if(_model.ChaseStatus == AIState.Chase && PlayerIsStillCleared())
        {
            SetPatrolMode();
        }

        if (_model.ChaseStatus == AIState.Patrol && !PlayerIsStillCleared() && !PlayerIsBeingChecked() && !PlayerOffenseIsRemembered() && PlayerDetected())
        {
            SetPlayerApproachMode();
        }
        else if ((_model.ChaseStatus == AIState.FindGhost || (_model.ChaseStatus == AIState.Patrol && PlayerOffenseIsRemembered() && !PlayerIsStillCleared())) && PlayerDetected())
        {
            SetPlayerChaseMode();
        }
        else if (_model.ChaseStatus == AIState.Chase && !PlayerDetected())
        {
            SetFindGhostMode();
        }
        else if (_model.ChaseStatus == AIState.Approach && CloseToTarget() && !PlayerTooFast() && PlayerDetected())
        {
            // skip scanning for now
            GameController.Instance.Player.Model.TimeUntilClearIsForgotten = PLAYER_CLEAR_MEMORY_TIME;
            GameController.Instance.Player.Model.PlayerIsBeingChecked = false;
            SetPatrolMode();
            EventHandler<MessageSendingEventArgs> handler = MessageSendingEvent;
            if (handler != null)
            {
                handler(this, new MessageSendingEventArgs()
                {
                    type = MessageType.FreeToGo
                });
            }
        }
    }

    private bool PlayerIsStillCleared()
    {
        if (GameController.Instance.Player.Model.TimeUntilClearIsForgotten > 0)
        {
            return true;
        }
        return false;
    }

    private bool PlayerIsBeingChecked()
    {
        return GameController.Instance.Player.Model.PlayerIsBeingChecked;
    }

    private bool PlayerOffenseIsRemembered()
    {
        if (GameController.Instance.Player.Model.TimeUntilOffenseIsForgotten > 0)
        {
            return true;
        }
        return false;
    }

    private bool PlayerDetected()
    {
        float distance = Vector2.Distance(transform.position, GameController.Instance.Player.View.transform.position);
        
        // Heat Signature check 
        float heat = GameController.Instance.Player.Model.CurrentHeat;
        if (heat > 0)
        {
            float heatAdjustedDistance = distance / (HEAT_FACTOR * heat);
            float adjustedDistance = Mathf.Lerp(distance, heatAdjustedDistance, LERP_FACTOR);
            if (adjustedDistance < SPOTTING_DISTANCE)
            {
                return true;
            }
        }
        

        return false;
    }

    private bool CloseToTarget()
    {
        float distance = Vector2.Distance(transform.position, _destinationSetter.target.position);
        if (distance <= STOP_DISTANCE_APPROACH)
        {
            return true;
        }
        return false;
    }

    private bool PlayerTooFast()
    {
        return GameController.Instance.Player.Model.CurrentSpeed > 0.03f;
    }

    private void CheckIfPlayerStopped()
    {
        _timeUntilPlayerMustBeStopped -= Time.deltaTime;
        if (_timeUntilPlayerMustBeStopped <= 0)
        {
            if (PlayerTooFast())
            {
                // player has not stopped, commence radical police actions
                SetPlayerChaseMode();
                GameController.Instance.Player.Model.PlayerIsBeingChecked = false;
            }
        }
    }
}
