using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();
    private Label message;
    private Label scoreLabel;
    private Timer messageTimer;
    public override void _Ready()
    {
        message = GetNode<Label>("Message");
        scoreLabel = GetNode<Label>("ScoreLabel");
        messageTimer = GetNode<Timer>("MessageTimer");
    }

    public void ShowMessage(String text)
    {
        message.Text = text;
        message.Show();
    }
    async public void ShowGameOver()
    {

        ShowMessage("Game Over!");
        messageTimer.Start();
        await ToSignal(messageTimer, Timer.SignalName.Timeout);
        ShowMessage("Flappy Bird");
        GetNode<Button>("StartButton").Show();
        scoreLabel.Hide();
    }
    public void UpdateScore(int score)
    {
        scoreLabel.Text = (score/2).ToString();
    }

    public void OnStartButtonPressed()
    {
        message.Hide();
        scoreLabel.Show();
        GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.StartGame);

    }
}
