

using System;

using var game = new BrickShooter.BrickShooterGame();
try
{
    game.Run();
}
catch(Exception e)
{
    game.Dispose();
    throw;
}
