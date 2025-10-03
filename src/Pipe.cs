using Godot;
using System;

public partial class Pipe : Area2D
{
    public void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}
