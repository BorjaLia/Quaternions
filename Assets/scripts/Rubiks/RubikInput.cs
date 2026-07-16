using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public struct RubikCommand
{
    public Vec3 axis;
    public float slice;
    public float angle;
}

public class RubikInput : MonoBehaviour
{
    [SerializeField] private bool isPositiveRotation = true;

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
        if (Input.GetKeyDown(KeyCode.W)) //X
        {
            EnqueueCommand(new Vec3(1, 0, 0), currentSlice, currentAngle);
        }
        else if (Input.GetKeyDown(KeyCode.A)) //Y
        {
            EnqueueCommand(new Vec3(0, 1, 0), currentSlice, currentAngle);
        }
        else if (Input.GetKeyDown(KeyCode.D)) //Z
        {
            EnqueueCommand(new Vec3(0, 0, 1), currentSlice, currentAngle);
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

        string faceName = GetFaceName(axis, slice);
        Debug.Log($"Move: Face {faceName}, Axis {axis}, Angle {angle}");
    }

    private string GetFaceName(Vec3 axis, float slice)
    {
        if (axis.x != 0) return slice > 0 ? "Right" : "Left";
        if (axis.y != 0) return slice > 0 ? "Up" : "Down";
        if (axis.z != 0) return slice > 0 ? "Front" : "Back";
        return "?";
    }
}