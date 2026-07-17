using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public struct RubikCommand
{
    public Vec3 axis;
    public float slice;
    public float angle;

    public override string ToString()
    {
        string face = "";
        bool isClockwise = angle > 0;

        if (axis.x != 0)
        {
            face = slice > 0 ? "R" : "L";
            if (slice < 0) isClockwise = !isClockwise;
        }
        else if (axis.y != 0)
        {
            face = slice > 0 ? "U" : "D";
            if (slice < 0) isClockwise = !isClockwise;
        }
        else if (axis.z != 0)
        {
            face = slice > 0 ? "F" : "B";
            if (slice < 0) isClockwise = !isClockwise;
        }

        return $"{face}{(isClockwise ? "" : "'")}";
    }

    public void Print()
    {
        Debug.Log(ToString());
    }
}

public class RubikInput : MonoBehaviour
{
    [SerializeField] private bool isPositiveRotation = true;

    [SerializeField] private int scrambleMoves = 20;

    //Queue of move commands
    public Queue<RubikCommand> commandQueue { get; private set; } = new Queue<RubikCommand>();

    void Update()
    {
        //Toggle dir (tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isPositiveRotation = !isPositiveRotation;
            Debug.Log($"{(isPositiveRotation ? "positive" : "negative")}");
        }

        //Change face (Shift)
        bool isShiftDown = Input.GetKey(KeyCode.LeftShift);
        float currentSlice = isShiftDown ? -1f : 1f;

        float currentAngle = isPositiveRotation ? 90f : -90f;

        //Axis (WAD)
        if (Input.GetKeyDown(KeyCode.R)) //X
        {
            EnqueueCommand(new Vec3(1, 0, 0), currentSlice, currentAngle);
        }
        else if (Input.GetKeyDown(KeyCode.U)) //Y
        {
            EnqueueCommand(new Vec3(0, 1, 0), currentSlice, currentAngle);
        }
        else if (Input.GetKeyDown(KeyCode.F)) //Z
        {
            EnqueueCommand(new Vec3(0, 0, 1), currentSlice, currentAngle);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ScrambleCube();
        }
    }

    private void ScrambleCube()
    {
        Vec3[] axes = new Vec3[] { new Vec3(1, 0, 0), new Vec3(0, 1, 0), new Vec3(0, 0, 1) };
        float[] slices = new float[] { 1f, -1f };
        float[] angles = new float[] { 90f, -90f };

        for (int i = 0; i < scrambleMoves; i++)
        {
            Vec3 randomAxis = axes[Random.Range(0, axes.Length)];
            float randomSlice = slices[Random.Range(0, slices.Length)];
            float randomAngle = angles[Random.Range(0, angles.Length)];

            EnqueueCommand(randomAxis, randomSlice, randomAngle);
        }
    }

    private void EnqueueCommand(Vec3 axis, float slice, float angle)
    {
        RubikCommand newCommand = new RubikCommand
        {
            axis = axis,
            slice = slice,
            angle = angle
        };

        commandQueue.Enqueue(newCommand);

        newCommand.Print();
    }
}