using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    [JsonProperty]
    public MicrobeColony Colony;

    [JsonProperty]
    public bool CanMasticate = false;

    [JsonProperty]
    public float FrustrationThreshold = 100.0f;

    [JsonProperty]
    private Vector3? migrationLocation;

    [JsonProperty]
    private EntityReference<Microbe> toxinPursuitTarget = new();

    [JsonProperty]
    private float toxinPursuitFrustration = 0.0f;

    [JsonProperty]
    private EntityReference<Microbe> masticationTarget = new();

    [JsonProperty]
    private float masticationFrustration = 0.0f;

    [JsonProperty]
    private float targetAngle;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
        Colony.ColonyMembers.ForEach(member =>
            member.State = Microbe.MicrobeState.Normal);
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        // Set the next migration goal, even though this might get overwritten
        if (migrationLocation == null || SquaredDistanceFromMe(migrationLocation.Value) < 100.0f)
        {
            SetNewRandomMovementDirection(random);
        }

        WanderToNewPosition(response, random, data);

        // If there is an existing strategy, try sticking witih it
        if (ExistingStrategy())
        {
            RunExistingStrategy(response, random);
            return response;
        }
        else
        {
            ChooseNewStrategy(response, random, data);
        }

        return response;
    }

    public bool ExistingStrategy()
    {
        var retval = false;

        if (toxinPursuitFrustration >= FrustrationThreshold)
        {
            toxinPursuitTarget.Value = null;
        }

        if (masticationFrustration >= FrustrationThreshold)
        {
            masticationTarget.Value = null;
        }

        if (masticationTarget.Value != null)
        {
            retval = true;
        }
        else if (masticationFrustration > 0.0f)
        {
            masticationFrustration -= 10.0f;
        }

        if (toxinPursuitTarget.Value != null)
        {
            retval = true;
        }
        else if (toxinPursuitFrustration > 0.0f)
        {
            toxinPursuitFrustration -= 5.0f;
        }

        return retval;
    }

    public void RunExistingStrategy(MulticellAIResponse response, Random random)
    {
        if (masticationTarget.Value != null)
        {
            if ((Colony.Master.LookAtPoint - masticationTarget.Value.GlobalTransform.origin).LengthSquared() < 100.0f)
            {
                if (random.NextFloat() > 0.5f)
                {
                    Turn(response, 1.2f);
                }
                else
                {
                    Turn(response, -1.2f);
                }
            }
            else
            {
                response.LookAt = masticationTarget.Value.GlobalTransform.origin;
            }

            masticationFrustration += 5.0f;

            if (masticationTarget.Value.Dead
                || SquaredDistanceFromMe(masticationTarget.Value.GlobalTransform.origin) < 100.0f
                || SquaredDistanceFromMe(masticationTarget.Value.GlobalTransform.origin) > 2000.0f)
            {
                masticationFrustration += FrustrationThreshold;
            }

            return;
        }

        if (toxinPursuitTarget.Value != null)
        {
            if (toxinPursuitTarget.Value.Dead)
            {
                toxinPursuitFrustration += FrustrationThreshold;
            }
            else
            {
                response.LookAt = toxinPursuitTarget.Value.GlobalTransform.origin;
                MoveTowards(response, toxinPursuitTarget.Value.GlobalTransform.origin);
                response.FireToxinAt = toxinPursuitTarget.Value.GlobalTransform.origin;
                toxinPursuitFrustration += 10;
            }
        }
    }

    public void ChooseNewStrategy(MulticellAIResponse response, Random random, MicrobeAICommonData data)
    {
        var microbesToEat = MicrobesToEat(data);
        CanMasticate = Colony.ColonyMembers.Any(member =>
            member.HasForwardPilus());

        if (microbesToEat.Count > 0)
        {
            if (toxinPursuitFrustration <= 0.0f
                && Colony.Master.Compounds.GetCompoundAmount(SimulationParameters.Instance.GetCompound("oxytoxy"))
                > 4.0f)
            {
                toxinPursuitTarget.Value = microbesToEat.First();
            }
            else if (CanMasticate && masticationFrustration <= 0.0f)
            {
                masticationTarget.Value = microbesToEat.First();
                masticationFrustration = 1.0f;
            }
        }
        else
        {
            var chunksToEat = ChunksNearMeWorthEating(data);
            if (chunksToEat.Count > 0)
            {
                Turn(response, 0.5f);
                MoveTowards(response, chunksToEat.First().GlobalTransform.origin);
            }
        }
    }

    private void WanderToNewPosition(MulticellAIResponse response, Random random, MicrobeAICommonData data)
    {
        response.LookAt = migrationLocation;

        if (migrationLocation != null)
        {
            MoveTowards(response, migrationLocation.Value);
        }
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

    private void MoveTowards(MulticellAIResponse response, Vector3 target)
    {
        if (response.LookAt != null)
        {
            var relativeLook = response.LookAt.Value - Colony.Master.GlobalTransform.origin;
            var lookAngle = Mathf.Atan2(relativeLook.z, relativeLook.x);

            var relativeMove = target - Colony.Master.GlobalTransform.origin;
            var moveAngle = Mathf.Atan2(relativeMove.z, relativeMove.x);

            // This calculation needs to subtract PI or else the organism is 90 degrees off target. I don't know why.
            var newAngle = moveAngle - lookAngle - 3.141592f / 2;

            response.MoveTowards = new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle));
        }
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
        && !microbe.Dead
        && SquaredDistanceFromMe(microbe.GlobalTransform.origin) < 1500.0f).ToList();
    }

    private float SquaredDistanceFromMe(Vector3 target)
    {
        return (target - Colony.Master.GlobalTransform.origin).LengthSquared();
    }
}
