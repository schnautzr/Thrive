using System;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    public MicrobeColony Colony;

    [JsonProperty]
    private Vector3? memorizedMoveDirection;

    [JsonProperty]
    private Double targetAngle;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        Turn(response, 0.1f);

        if (memorizedMoveDirection == null)
        {
            SetNewRandomMovementDirection(random);
        }

        if (memorizedMoveDirection != null)
        {
            response.MoveTowards = memorizedMoveDirection;
        }

        return response;
    }

    private void SetNewRandomMovementDirection(Random random)
    {
        var randomXTarget = random.Next(-1000.0f, 1000.0f);
        var randomYTarget = random.Next(-1000.0f, 1000.0f);

        memorizedMoveDirection = new Vector3(randomXTarget, randomYTarget, -Constants.AI_BASE_MOVEMENT);
    }

    private void Turn(MulticellAIResponse response, float turn)
    {
        var offsetPosition = Colony.Master.LookAtPoint - Colony.Master.GlobalTransform.origin;
        var previousAngle = Mathf.Atan2(offsetPosition.y, offsetPosition.x);

        response.LookAt = Colony.Master.GlobalTransform.origin
            + new Vector3(Mathf.Cos(previousAngle + turn) * 1000.0f,
                0,
                Mathf.Sin(previousAngle + turn) * 1000.0f);
    }
}
