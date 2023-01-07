using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{
    public Direction direction;
    public Rotation rotation;
}

public enum Direction
{
    Front,
    Back
}

public enum Rotation
{
    None,
    Left,
    Right
}
