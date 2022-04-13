using System;

public class OrganismAI
{
    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        response.State = Microbe.MicrobeState.Engulf;

        return response;
    }
}
