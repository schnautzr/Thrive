using System;
using Godot;
using Newtonsoft.Json;

public class OrganismAI
{
    public MicrobeColony Colony;

    [JsonProperty]
    private Vector3? memorizedMoveDirection;

    [JsonProperty]
    private float targetAngle;

    public OrganismAI(MicrobeColony colony)
    {
        Colony = colony;
    }

    public MulticellAIResponse OrganismBehavior(float delta, Random random, MicrobeAICommonData data)
    {
        var response = new MulticellAIResponse();

        Turn(response, 0.1f);

        if (memorizedMoveDirection == null)
        {
            SetNewRandomMovementDirection(random);
        }

        if (memorizedMoveDirection != null)
        {
            MoveTowards(response, ThePlayer(data));
        }

        return response;
    }

    private void SetNewRandomMovementDirection(Random random)
    {
        var randomXTarget = random.Next(-1000.0f, 1000.0f);
        var randomYTarget = random.Next(-1000.0f, 1000.0f);

        memorizedMoveDirection = new Vector3(0, 0, -Constants.AI_BASE_MOVEMENT);
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

        var newAngle = moveAngle - lookAngle - 3.141592f / 2;

        response.MoveTowards = new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle));
    }

    //TODO: Remove
    private Vector3 ThePlayer(MicrobeAICommonData data)
    {
        foreach (Microbe microbe in data.AllMicrobes)
        {
            if (microbe.IsPlayerMicrobe)
            {
                return microbe.GlobalTransform.origin;
            }
        }

        throw new Exception();
    }
}