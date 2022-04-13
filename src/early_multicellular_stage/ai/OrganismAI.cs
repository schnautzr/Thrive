using System;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    public MicrobeColony Colony;

    [JsonProperty]
    private Vector3? memorizedMoveDirection;

    [JsonProperty]
    private float targetAngle;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        Turn(response, random.NextFloat() * 0.3f);

        if (memorizedMoveDirection == null)
        {
            SetNewRandomMovementDirection(random);
        }

        if (memorizedMoveDirection != null)
        {
            MoveTowards(response, memorizedMoveDirection);
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
        targetAngle += turn;

        response.LookAt = Colony.Master.GlobalTransform.origin
            + new Vector3(Mathf.Cos(targetAngle) * 1000.0f,
                0,
                Mathf.Sin(targetAngle) * 1000.0f);
    }

    private void MoveTowards(MulticellAIResponse response, Vector3? target)
    {
        response.MoveTowards = target - response.LookAt;
    }
}