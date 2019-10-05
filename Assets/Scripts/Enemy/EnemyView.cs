using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyView : MonoBehaviour
{
    private const float MAX_SPEED_PATROL = 1f;
    private const float MAX_SPEED_GHOST = 2f;
    private const float MAX_SPEED_CHASE = 3.5f;
    private const float MAX_ROTATION_SPEED_PATROL = 300f;
    private const float MAX_ROTATION_SPEED_GHOST = 330f;
    private const float MAX_ROTATION_SPEED_CHASE = 360f;
    private const float MAX_ACCELERATION_PATROL = 6f;
    private const float MAX_ACCELERATION_GHOST = 8f;
    private const float MAX_ACCELERATION_CHASE = 10f;
    private const float SLOWDOWN_DISTANCE_PATROL = 0.6f;
    private const float SLOWDOWN_DISTANCE_GHOST = 1f;
    private const float SLOWDOWN_DISTANCE_CHASE = 0f;
    
    private const float HEAT_FACTOR = 5f;
    private const float LERP_FACTOR = 0.8f;
    private const float SPOTTING_DISTANCE = 5f;

    private Enemy _model;
    [SerializeField] private GameObject _enemyShip;
    [SerializeField] private AIDestinationSetter _destinationSetter;
    [SerializeField] private AIPath _aiPath;

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
    }

    public void SetPlayerChaseMode()
    {
        if (_destinationSetter.target != null && _destinationSetter.target.name == "Waypoint")
        {
            Destroy(_destinationSetter.target.gameObject);
        }

        _destinationSetter.target = GameController.Instance.Player.View.transform;
        _aiPath.maxSpeed = MAX_SPEED_CHASE;
        _aiPath.rotationSpeed = MAX_ROTATION_SPEED_CHASE;
        _aiPath.slowdownDistance = SLOWDOWN_DISTANCE_CHASE;
        _aiPath.maxAcceleration = MAX_ACCELERATION_CHASE;
        _model.ChaseStatus = AIState.Chase;
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
                SetPatrolMode();
            }
        }

        if ((_model.ChaseStatus == AIState.Patrol || _model.ChaseStatus == AIState.FindGhost) && PlayerDetected())
        {
            SetPlayerChaseMode();
        }
        else if (_model.ChaseStatus == AIState.Chase && !PlayerDetected())
        {
            SetFindGhostMode();
        }
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
                if(_model.ChaseStatus == AIState.Patrol)
                    Debug.Log("Agent detected player! Distance: " + distance + " | Heat-Adjusted distance: " + heatAdjustedDistance + " | Final distance: " + adjustedDistance);
                return true;
            }
        }
        

        return false;
    }
}
