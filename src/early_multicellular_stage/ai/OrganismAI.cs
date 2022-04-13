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

        response.LookAt = new Vector3(1, 1, 1);
        response.MoveTowards = new Vector3(1, 1, -Constants.AI_BASE_MOVEMENT);

        return response;
    }
}
