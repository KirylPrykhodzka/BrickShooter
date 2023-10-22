using BrickShooter.Drawing;
using Moq;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace BrickShooter.Tests.Drawing
{
    [TestFixture]
    public class DrawingSystemTests
    {
        [Test]
        public void Run_CallsDrawMethodOnAllDrawableObjects()
        {
            // Arrange
            var drawingSystem = new DrawingSystem();
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            var mockDrawables = fixture.CreateMany<Mock<IDrawableObject>>();
            var drawables = mockDrawables.Select(mock => mock.Object).ToList();

            foreach (var drawable in drawables)
            {
                drawingSystem.Register(drawable);
            }

            // Act
            drawingSystem.Run();

            // Assert
            foreach (var mockDrawable in mockDrawables)
            {
                mockDrawable.Verify(d => d.Draw(), Times.Once);
            }
        }

        [Test]
        public void Run_DoesNotDrawUnregisteredObjects()
        {
            // Arrange
            var drawingSystem = new DrawingSystem();
            var drawableObjectMock = new Mock<IDrawableObject>();
            drawingSystem.Register(drawableObjectMock.Object);
            drawingSystem.Unregister(drawableObjectMock.Object);

            // Act
            drawingSystem.Run();

            // Assert
            drawableObjectMock.Verify(d => d.Draw(), Times.Never);
        }

        [Test]
        public void Run_UnregistersAllObjectsAfterReset()
        {
            // Arrange
            var drawingSystem = new DrawingSystem();
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            var mockDrawables = fixture.CreateMany<Mock<IDrawableObject>>();
            var drawables = mockDrawables.Select(mock => mock.Object).ToList();

            foreach (var drawable in drawables)
            {
                drawingSystem.Register(drawable);
            }

            drawingSystem.Reset();

            // Act
            drawingSystem.Run();

            // Assert
            foreach (var mockDrawable in mockDrawables)
            {
                mockDrawable.Verify(d => d.Draw(), Times.Never);
            }
        }
    }
}
