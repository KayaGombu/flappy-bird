using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    [Signal]
    public delegate void DeletePipeEventHandler();
    [Export]
    private float PipeSpeed = 150f;
    private PackedScene scene = GD.Load<PackedScene>("res://src/pipe.tscn");
    private float GroundLevel = 573.0f;
    private float gapSize = 150;
    private int score = 0;
    private bool isStart = false;
    private const float PipeWidth = 46.513f;
    private const float HeadHeight = 24f;
    private List<Pipe> pipeList;
    private CharacterBody2D bird;
    private Timer timer;
    private Hud hud;
    private Node2D ground;
    public override void _Ready()
    {
        bird = GetNode<CharacterBody2D>("Objects/Bird");
        pipeList = new List<Pipe>();
        timer = GetNode<Timer>("SpawnTimer");
        hud = GetNode<Hud>("HUD");
        ground = GetNode<Node2D>("Objects/Ground");
        ground.GetNode<VisibleOnScreenNotifier2D>("GroundNotifier").ScreenExited += OnGroundNotifierScreenExited;


    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (isStart)
            HandlePipeMovement(delta);

    }
    public void NewGame()
    {
        timer.Start();
        GetNode<Timer>("DifficultyTimer").Start();
        isStart = true;
        bird.Show();
        bird.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        //CreateGapPipes(100f, 750, 100f);
        //CreateGapPipes(300f, 950, 300);
    }
    public void GameOver()
    {
        foreach (Pipe pipe in pipeList)
        {
            pipe.QueueFree();       
        }
        pipeList.Clear();
        GD.Print(pipeList);
        isStart = false;
        timer.Stop();
        score = 0;
        hud.ShowGameOver();
    }


    private void HandlePipeMovement(double delta)
    {
        bool inFront;

        foreach (Pipe pipe in pipeList)
        {
            inFront = pipe.Position.X > bird.Position.X;
            pipe.Position += Vector2.Left * PipeSpeed * (float)delta;
            if (inFront && pipe.Position.X <= bird.Position.X)
            {
                score++;
                hud.UpdateScore(score);
            }
        }
        
        ground.Position += Vector2.Left * PipeSpeed * (float)delta;
    }

    private void CreateGapPipes(float gapSize, float xPos, float yPos)
    {
        CreatePipe(xPos, yPos - gapSize * .5f - HeadHeight, false);
        CreatePipe(xPos, GroundLevel - yPos - gapSize * .5f - HeadHeight, true);
    }

    private void CreatePipe(float xPosition, float height, bool onTop)
    {

        var pipe = (Pipe)scene.Instantiate();
        var notif = (VisibleOnScreenNotifier2D)pipe.GetNode("VisibleOnScreenNotifier2D");
        GetNode("Objects").AddChild(pipe);

        //Connect BodyEntered signl
        pipe.BodyEntered += OnPipeBodyEntered;
        notif.ScreenExited += OnVisibleOnScreenNotifier2DScreenExited;


        //Get Nodes of Pipe
        var body = (Sprite2D)pipe.GetNode("PipeBody");
        var collider = (CollisionShape2D)pipe.GetNode("PipeCollider");
        var head = (Sprite2D)pipe.GetNode("PipeHead");

        //Set size of pipe body
        body.SetScale(new Vector2(1, height / body.GetRect().Size.Y));
        var bHeight = (body.GetRect().Size * body.Scale).Y;

        //Set size of pipe collider
        var cSize = new RectangleShape2D();
        cSize.Size = new Vector2(PipeWidth, bHeight + HeadHeight);
        collider.Shape = cSize;

        //Set position of Pipe
        pipe.Position = new Vector2(xPosition, GroundLevel);
        head.Position = new Vector2(0, -bHeight);
        collider.Position = new Vector2(0, -(HeadHeight + bHeight) / 2);

        if (onTop)
        {
            pipe.RotationDegrees = 180;
            pipe.Position = new Vector2(xPosition, 0);
        }

        pipeList.Add(pipe);

    }
    public void OnPipeBodyEntered(Node2D body)
    {
        if (body.Equals(bird))
        {
            GD.Print("Hit");
            bird.Hide();
            GameOver();

        }
    }
    public void OnSpawnTimerTimeout()
    {
        float minHeight = gapSize * .5f + 40f;
        float maxHeight = GroundLevel - minHeight;
        CreateGapPipes(gapSize, GetViewportRect().Size.X + PipeWidth / 2, (float)GD.RandRange(minHeight, maxHeight));

    }
    public void OnDifficultyTimerTimeout()
    {
        if (timer.WaitTime > 1)
        {
           // timer.WaitTime -= 0.2;
           // gapSize -= 20;
        }
    }
    public void OnVisibleOnScreenNotifier2DScreenExited()
    {
        pipeList.RemoveAt(0);
    }
    public void OnGroundNotifierScreenExited()
    {
        ground.Position = new Vector2(0, 0);
    }
}
