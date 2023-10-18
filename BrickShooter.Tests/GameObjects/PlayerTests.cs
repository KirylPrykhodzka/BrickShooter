using AutoFixture;
using AutoFixture.NUnit3;
using BrickShooter.Constants;
using BrickShooter.Input;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moq;
using NUnit.Framework;

namespace BrickShooter.GameObjects.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private IFixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
        }

        [Test]
        public void Player_Initialization_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            Vector2 initialPosition = fixture.Create<Vector2>();

            // Act
            var player = new Player(initialPosition);

            // Assert
            player.Position.Should().Be(initialPosition);
            player.InitialBulletPosition.Should().Be(player.Position + PlayerConstants.BARREL_TIP_OFFSET);
        }

        [Test]
        public void Update_PlayerNotMovingOnY_When_W_And_S_KeysArePressed_ShouldNotMoveY()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), 0))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.W, Keys.S });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(0);
        }

        [Test]
        public void Update_PlayerNotMovingOnY_When_W_And_S_KeysAreNotPressed_ShouldNotMoveY()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), 0))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(fixture
                .Create<Generator<Keys>>()
                .Where(x => x != Keys.W && x != Keys.S)
                .Take(3)
                .ToArray());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(0);
        }

        [Test]
        public void Update_PlayerNotMovingOnY_When_W_KeyIsPressed_ShouldAccelerateUp()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), 0))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.W });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(-PlayerConstants.ACCELERATION_FACTOR * PlayerConstants.MAX_VELOCITY);
        }

        [Test]
        public void Update_PlayerNotMovingOnY_When_S_KeyIsPressed_ShouldAccelerateDown()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), 0))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.S });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(PlayerConstants.ACCELERATION_FACTOR * PlayerConstants.MAX_VELOCITY);
        }

        [Test]
        public void Update_PlayerNotMovingOnX_When_A_And_D_KeysArePressed_ShouldNotMoveX()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(0, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.A, Keys.D });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(0);
        }

        [Test]
        public void Update_PlayerNotMovingOnX_When_A_And_D_KeysAreNotPressed_ShouldNotMoveX()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(0, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(fixture
                .Create<Generator<Keys>>()
                .Where(x => x != Keys.A && x != Keys.D)
                .Take(3)
                .ToArray());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_A_KeyIsPressed_ShouldAccelerateLeft()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.A });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(-PlayerConstants.ACCELERATION_FACTOR * PlayerConstants.MAX_VELOCITY);
            player.Velocity.Y.Should().Be(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_D_KeyIsPressed_ShouldAccelerateRight()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.D });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(PlayerConstants.ACCELERATION_FACTOR * PlayerConstants.MAX_VELOCITY);
            player.Velocity.Y.Should().Be(0);
        }

        [Test]
        public void Update_PlayerMovingUp_When_W_KeyIsNotPressed_ShouldDecelerate()
        {
            // Arrange
            var velocityY = new Random().Next(-(int)PlayerConstants.MAX_VELOCITY, -(int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR));
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), velocityY))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(Array.Empty<Keys>());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(velocityY - velocityY * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingDown_When_S_KeyIsNotPressed_ShouldDecelerate()
        {
            // Arrange
            var velocityY = new Random().Next((int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR), (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), velocityY))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(Array.Empty<Keys>());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(velocityY - velocityY * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingLeft_When_A_KeyIsNotPressed_ShouldDecelerate()
        {
            // Arrange
            var velocityX = new Random().Next(-(int)PlayerConstants.MAX_VELOCITY, -(int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR));
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(velocityX, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(Array.Empty<Keys>());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(velocityX - velocityX * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingRight_When_D_KeyIsNotPressed_ShouldDecelerate()
        {
            // Arrange
            var velocityX = new Random().Next((int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR), (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(velocityX, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(Array.Empty<Keys>());
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(velocityX - velocityX * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingOnXAxis_When_A_And_D_KeysArePressed_ShouldDecelerate()
        {
            // Arrange
            var velocityX = new Random().Next((int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR), (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(velocityX, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.A, Keys.D });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(velocityX - velocityX * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingOnYAxis_When_W_And_S_KeysArePressed_ShouldDecelerate()
        {
            // Arrange
            var velocityY = new Random().Next((int)(PhysicsConstants.MIN_VELOCITY / PlayerConstants.DECELERATION_FACTOR), (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), velocityY))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.W, Keys.S });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(velocityY - velocityY * PlayerConstants.DECELERATION_FACTOR);
        }

        [Test]
        public void Update_PlayerMovingLeft_When_D_KeyIsPressed_ShouldStopImmediately()
        {
            // Arrange
            var velocityX = new Random().Next(-(int)PlayerConstants.MAX_VELOCITY, -PhysicsConstants.MIN_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(velocityX, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.D });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(0);
        }

        [Test]
        public void Update_PlayerMovingRight_When_A_KeyIsPressed_ShouldStartMovingLeftImmediately()
        {
            // Arrange
            var velocityX = new Random().Next(PhysicsConstants.MIN_VELOCITY, (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(velocityX, fixture.Create<int>()))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.A });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().Be(0);
        }

        [Test]
        public void Update_PlayerMovingUp_When_S_KeyIsPressed_ShouldStartMovingDownImmediately()
        {
            // Arrange
            var velocityY = new Random().Next(-(int)PlayerConstants.MAX_VELOCITY, -PhysicsConstants.MIN_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), velocityY))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.S });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(0);
        }

        [Test]
        public void Update_PlayerMovingDown_When_W_KeyIsPressed_ShouldStartMovingUpImmediately()
        {
            // Arrange
            var velocityY = new Random().Next(PhysicsConstants.MIN_VELOCITY, (int)PlayerConstants.MAX_VELOCITY);
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, new Vector2(fixture.Create<int>(), velocityY))
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new Keys[] { Keys.W });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().Be(0);
        }

        [Test, AutoData]
        public void Update_ShouldRotateWhenMousePositionChanges([Range(0, Math.PI, 0.1)] double rotation)
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Position, fixture.Create<Vector2>())
                .With(x => x.Rotation, rotation)
                .Create();
            var mouseState = new Mock<IMouseState>();
            mouseState.SetupGet(x => x.X).Returns(fixture.Create<int>());
            mouseState.SetupGet(x => x.Y).Returns(fixture.Create<int>());
            var expectedRotation = (float)Math.Atan2(mouseState.Object.Y - player.Position.Y, mouseState.Object.X - player.Position.X);

            // Act
            GlobalObjects.MouseState = mouseState.Object;
            GlobalObjects.KeyboardState = new Mock<IKeyboardState>().Object;
            player.Update();

            //Assert
            player.Rotation.Should().Be(expectedRotation);
            player.DidRotate.Should().Be(expectedRotation != rotation);
        }
    }
}
