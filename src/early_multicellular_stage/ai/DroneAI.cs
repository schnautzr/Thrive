using System;
using Godot;

public class DroneAI
{
    public static MulticellAIResponse DroneBehavior(Microbe microbe, Random random, MicrobeAICommonData data)
    {
        var behavior = new MulticellAIResponse();

        if (HasNearbyEngulfableChunks(microbe, data))
        {
            behavior.State = Microbe.MicrobeState.Engulf;
        }
        else
        {
            behavior.State = Microbe.MicrobeState.Normal;
        }

        return behavior;
    }

    private static bool HasNearbyEngulfableChunks(Microbe microbe, MicrobeAICommonData data)
    {
        foreach (var chunk in data.AllChunks)
        {
            if (DistanceFromMe(microbe, chunk.Translation) < microbe.Radius * 80)
            {
                return true;
            }
        }

        return false;
    }

    private static float DistanceFromMe(Microbe microbe, Vector3 target)
    {
        return (target - microbe.GlobalTransform.origin).LengthSquared();
    }
}
