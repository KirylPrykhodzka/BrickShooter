using AutoFixture;
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
        public void Update_PlayerNotMoving_When_W_KeyIsPressed_ShouldStartMovingUp()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.W });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().BeLessThan(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_S_KeyIsPressed_ShouldStartMovingDown()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.S });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.Y.Should().BeGreaterThan(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_W_And_S_KeysArePressed_ShouldNotMoveY()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
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
        public void Update_PlayerNotMoving_When_W_And_S_KeysAreNotPressed_ShouldNotMoveY()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
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
        public void Update_PlayerNotMoving_When_A_KeyIsPressed_ShouldStartMovingLeft()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.A });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().BeLessThan(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_D_KeyIsPressed_ShouldStartMovingRight()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
                .Create();
            var keyboardState = new Mock<IKeyboardState>();
            keyboardState.SetupGet(x => x.PressedKeys).Returns(new[] { Keys.D });
            GlobalObjects.KeyboardState = keyboardState.Object;

            // Act
            player.Update();

            // Assert
            player.Velocity.X.Should().BeGreaterThan(0);
        }

        [Test]
        public void Update_PlayerNotMoving_When_A_And_D_KeysArePressed_ShouldNotMoveX()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
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
        public void Update_PlayerNotMoving_When_A_And_D_KeysAreNotPressed_ShouldNotMoveX()
        {
            // Arrange
            var player = fixture.Build<Player>()
                .With(x => x.Velocity, Vector2.Zero)
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
        public void Update_ShouldHandleRotationInput()
        {
            // Arrange
            var player = fixture.Create<Player>();
            var mouseState = new Mock<IMouseState>();

            // Act
            GlobalObjects.MouseState = mouseState.Object;
            player.Update();

            // Assert: Add appropriate assertions based on the expected behavior
        }
    }
}
