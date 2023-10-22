using AutoFixture;
using Moq;
using NUnit.Framework;
using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Models;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
using BrickShooter.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrickShooter.Tests.GameObjects
{
    [TestFixture]
    public class LevelTests
    {
        private Mock<IPool<Bullet>> bulletPoolMock;
        private Mock<IPhysicsSystem> physicsSystemMock;
        private Mock<IDrawingSystem> drawingSystemMock;
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            bulletPoolMock = new Mock<IPool<Bullet>>();
            physicsSystemMock = new Mock<IPhysicsSystem>();
            drawingSystemMock = new Mock<IDrawingSystem>();
            fixture = new Fixture();
        }

        [Test]
        public void Load_ShouldLoadLevelData()
        {
            // Arrange
            var level = new Level(bulletPoolMock.Object, physicsSystemMock.Object, drawingSystemMock.Object);
            var levelName = fixture.Create("levelName");
            var levelData = fixture.Create<LevelData>();
            var contentMock = new Mock<IContentManager>();
            var viewportBounds = fixture.Create<Rectangle>();
            contentMock.Setup(x => x.Load<LevelData>($"Levels/{levelName}")).Returns(levelData);
            GlobalObjects.Content = contentMock.Object;

            // Act
            level.Load(levelName, viewportBounds);

            //Assert
            contentMock.Verify(x => x.Load<LevelData>($"Levels/{levelName}"), Times.Once);
        }

        [Test]
        public void Load_ShouldInitializeBackground()
        {
            // Arrange
            var level = new Level(bulletPoolMock.Object, physicsSystemMock.Object, drawingSystemMock.Object);
            var levelName = fixture.Create("levelName");
            var levelData = fixture.Create<LevelData>();
            var contentMock = new Mock<IContentManager>();
            var viewportBounds = fixture.Create<Rectangle>();
            contentMock.Setup(x => x.Load<LevelData>($"Levels/{levelName}")).Returns(levelData);
            GlobalObjects.Content = contentMock.Object;

            // Act
            level.Load(levelName, viewportBounds);

            //Assert
            contentMock.Verify(x => x.Load<Texture2D>($"Backgrounds/{levelData.BackgroundTextureName}"), Times.Once);
            drawingSystemMock.Verify(x => x.Register(It.IsAny<Background>()), Times.Once);
        }

        [Test]
        public void Load_ShouldInitializePlayer()
        {
            // Arrange
            var level = new Level(bulletPoolMock.Object, physicsSystemMock.Object, drawingSystemMock.Object);
            var levelName = fixture.Create("levelName");
            var levelData = fixture.Create<LevelData>();
            var contentMock = new Mock<IContentManager>();
            var viewportBounds = fixture.Create<Rectangle>();
            contentMock.Setup(x => x.Load<LevelData>($"Levels/{levelName}")).Returns(levelData);
            GlobalObjects.Content = contentMock.Object;

            // Act
            level.Load(levelName, viewportBounds);

            //Assert
            drawingSystemMock.Verify(x => x.Register(It.IsAny<Player>()), Times.Once);
            physicsSystemMock.Verify(x => x.RegisterMobileObject(It.Is<Player>(x => x.Position == new Vector2(
                Math.Abs(viewportBounds.Width - levelData.Width) / 2 + levelData.InitialPlayerPosition.X,
                Math.Abs(viewportBounds.Height - levelData.Height) / 2 + levelData.InitialPlayerPosition.Y))),
                Times.Once);
        }

        [Test]
        public void Load_ShouldInitializeWalls()
        {
            // Arrange
            var level = new Level(bulletPoolMock.Object, physicsSystemMock.Object, drawingSystemMock.Object);
            var levelName = fixture.Create("levelName");
            var levelData = fixture.Create<LevelData>();
            var contentMock = new Mock<IContentManager>();
            var viewportBounds = fixture.Create<Rectangle>();
            contentMock.Setup(x => x.Load<LevelData>($"Levels/{levelName}")).Returns(levelData);
            GlobalObjects.Content = contentMock.Object;

            // Act
            level.Load(levelName, viewportBounds);

            //Assert
            foreach(var wallPlacement in levelData.Walls.Placements)
            {
                drawingSystemMock.Verify(x => x.Register(It.Is<Wall>(x => x.RectBounds == new Rectangle(
                    Math.Abs(viewportBounds.Width - levelData.Width) / 2 + (int)wallPlacement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                    Math.Abs(viewportBounds.Height - levelData.Height) / 2 + (int)wallPlacement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
                    levelData.Walls.TileWidth,
                    levelData.Walls.TileHeight))),
                    Times.Once);
                physicsSystemMock.Verify(x => x.RegisterImmobileObject(It.Is<Wall>(x => x.RectBounds == new Rectangle(
                    Math.Abs(viewportBounds.Width - levelData.Width) / 2 + (int)wallPlacement.X * levelData.Walls.TileWidth + levelData.Walls.OffsetX,
                    Math.Abs(viewportBounds.Height - levelData.Height) / 2 + (int)wallPlacement.Y * levelData.Walls.TileHeight + levelData.Walls.OffsetY,
                    levelData.Walls.TileWidth,
                    levelData.Walls.TileHeight))),
                    Times.Once);
            }
        }
    }
}
