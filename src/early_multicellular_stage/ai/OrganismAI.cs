using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    public MicrobeColony Colony;

    [JsonProperty]
    private Vector3? migrationLocation;

    [JsonProperty]
    private float targetAngle;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        if (migrationLocation == null || SquaredDistanceFromMe(migrationLocation.Value) < 100.0f)
        {
            WanderToNewPosition(response, random, data);
        }

        var chunksToEat = ChunksNearMeWorthEating(data);
        if (chunksToEat.Count > 0)
        {
            Turn(response, 0.5f);
            if (migrationLocation != null)
            {
                MoveTowards(response, chunksToEat.First().GlobalTransform.origin);
            }
        }
        else
        {
            var microbesToShoot = MicrobesToEat(data);

            if (microbesToShoot.Count > 0)
            {
                response.LookAt = microbesToShoot.First().GlobalTransform.origin;
                MoveTowards(response, migrationLocation);
                response.FireToxinAt = microbesToShoot.First().GlobalTransform.origin;
            }
        }

        return response;
    }

    private void WanderToNewPosition(MulticellAIResponse response, Random random, MicrobeAICommonData data)
    {
        SetNewRandomMovementDirection(random);
        response.LookAt = migrationLocation;
        MoveTowards(response, migrationLocation);
    }

    private void SetNewRandomMovementDirection(Random random)
    {
        var maxDistance = 200.0f;

        migrationLocation = Colony.Master.GlobalTransform.origin
            + new Vector3(random.Next(-maxDistance, maxDistance), 0, random.Next(-maxDistance, maxDistance));
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
        var relativeLook = response.LookAt - Colony.Master.GlobalTransform.origin;
        var lookAngle = Mathf.Atan2(relativeLook.Value.z, relativeLook.Value.x);

        var relativeMove = target - Colony.Master.GlobalTransform.origin;
        var moveAngle = Mathf.Atan2(relativeMove.Value.z, relativeMove.Value.x);

        // This calculation needs to subtract PI or else the organism is 90 degrees off target. I don't know why.
        var newAngle = moveAngle - lookAngle - 3.141592f / 2;

        response.MoveTowards = new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle));
    }

    private List<FloatingChunk> ChunksNearMeWorthEating(MicrobeAICommonData data)
    {
        return data.AllChunks.Where(chunk =>
            chunk.ContainedCompounds != null
            && SquaredDistanceFromMe(chunk.Translation) < 1000.0f).ToList();
    }

    private List<Microbe> MicrobesToEat(MicrobeAICommonData data)
    {
        return data.AllMicrobes.Where(microbe =>
        microbe.Species != Colony.Master.Species
        && SquaredDistanceFromMe(microbe.GlobalTransform.origin) < 1000.0f).ToList();
    }

    private float SquaredDistanceFromMe(Vector3 target)
    {
        return (target - Colony.Master.GlobalTransform.origin).LengthSquared();
    }
}
