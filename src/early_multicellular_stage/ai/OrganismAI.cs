using System;

public class OrganismAI
{
    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        response.MoveTowards = new Godot.Vector3(0, 0, 0);

        return response;
    }
}
