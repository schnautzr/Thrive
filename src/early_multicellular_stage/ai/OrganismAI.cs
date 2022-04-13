using System;
using Godot;

public class OrganismAI
{
    public MicrobeColony Colony;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        var randomXTarget = random.Next(-1000.0f, 1000.0f);
        var randomYTarget = random.Next(-1000.0f, 1000.0f);

        response.LookAt = new Vector3(0, 0, 0);
        response.MoveTowards = new Vector3(randomXTarget, randomYTarget, -Constants.AI_BASE_MOVEMENT);

        return response;
    }
}
