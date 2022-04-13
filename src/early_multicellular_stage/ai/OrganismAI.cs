using System;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    public MicrobeColony Colony;

    [JsonProperty]
    private Vector3? memorizedMoveDirection;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        response.LookAt = new Vector3(0, 0, 0);

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
}
