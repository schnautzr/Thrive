using System;
using Godot;

public class OrganismAI
{
    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        response.LookAt = new Vector3(1, 0, 0);
        response.MoveTowards = new Vector3(0, 0, -Constants.AI_BASE_MOVEMENT);

        return response;
    }
}
