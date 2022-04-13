using System;
using System.Linq;
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

        var prey = PreyToShoot(microbe, data);

        if (prey != null)
        {
            behavior.FireToxinAt = prey.GlobalTransform.origin - microbe.GlobalTransform.origin;
        }

        return behavior;
    }

    private static bool HasNearbyEngulfableChunks(Microbe microbe, MicrobeAICommonData data)
    {
        foreach (var chunk in data.AllChunks)
        {
            if (DistanceFromMe(microbe, chunk.Translation) < microbe.Radius * 80 &&
                CanEatChunk(microbe, chunk))
            {
                return true;
            }
        }

        return false;
    }

    private static Microbe? PreyToShoot(Microbe microbe, MicrobeAICommonData data)
    {
        Microbe? prey = null;
        foreach (var otherMicrobe in data.AllMicrobes)
        {
            if (otherMicrobe.Species != microbe.Species && !otherMicrobe.Dead
                && DistanceFromMe(microbe, otherMicrobe.GlobalTransform.origin) <
                    (250.0f * microbe.Species.Behaviour.Aggression / Constants.MAX_SPECIES_AGGRESSION))
                {
                    prey = otherMicrobe;
                }
        }

        return prey;
    }

    private static float DistanceFromMe(Microbe microbe, Vector3 target)
    {
        return (target - microbe.GlobalTransform.origin).LengthSquared();
    }

    private static bool CanEatChunk(Microbe microbe, FloatingChunk chunk)
    {
        return chunk.ContainedCompounds != null &&
            chunk.ContainedCompounds.Compounds.Any(x => microbe.Compounds.IsUseful(x.Key));
    }
}
