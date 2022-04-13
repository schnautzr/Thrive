using Godot;

public class MulticellAIResponse
{
    public Vector3? MoveTowards;
    public Vector3? LookAt;
    public Microbe.MicrobeState State = Microbe.MicrobeState.Normal;
    public Vector3? FireToxinAt;
}
