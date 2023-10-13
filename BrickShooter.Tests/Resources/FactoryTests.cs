using AutoFixture;
using BrickShooter.Resources;
using FluentAssertions;
using NUnit.Framework;

namespace BrickShooter.Tests.Resources
{
    [TestFixture]
    public class FactoryTests
    {
        private Fixture fixture;

        [SetUp]
        public void Setup()
        {
            fixture = new Fixture();
        }

        [Test]
        public void GetItem_ShouldReturnNewItemWhenEmpty()
        {
            // Arrange
            var factory = new Factory<ResetableObject>();

            // Act
            var item = factory.GetItem();

            // Assert
            item.Should().NotBeNull();
            item.Should().BeOfType<ResetableObject>();
        }

        [Test]
        public void GetItem_ShouldReturnPooledItemWhenAvailable()
        {
            // Arrange
            var factory = new Factory<ResetableObject>();
            var item1 = factory.GetItem();
            factory.Return(item1);

            // Act
            var item2 = factory.GetItem();

            // Assert
            item2.Should().NotBeNull();
            item2.Should().Be(item1);
        }

        [Test]
        public void Return_ShouldResetAndPoolItem()
        {
            // Arrange
            var factory = new Factory<ResetableObject>();
            var item = factory.GetItem();
            item.Value = fixture.Create<int>();

            // Act
            factory.Return(item);
            var pooledItem = factory.GetItem();

            // Assert
            pooledItem.Should().Be(item);
            pooledItem.Value.Should().Be(default); // Value should be reset to its default value
        }
    }    
}