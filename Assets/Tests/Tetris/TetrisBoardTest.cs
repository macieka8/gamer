using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;
using gamer.tetris;

namespace gamer.tetris.Tests
{
    public class TetrisBoardTest
    {
        TetrisBoard _tetrisBoard;
        Sprite _testSprite;

        [SetUp]
        public void Setup()
        {
            _testSprite = Sprite.Create(new Texture2D(1, 1), Rect.zero, Vector2.zero);
            _tetrisBoard = new TetrisBoard();
            _tetrisBoard.SetValue(0, 0, new Tile(_testSprite));
            _tetrisBoard.SetValue(9, 0, new Tile(_testSprite));
        }

        [Test]
        [TestCase(0, 0, false)]
        [TestCase(1, 0, true)]
        public void GetValue_Valid_Input_XY(int x, int y, bool expectedIsEmpty)
        {
            var value = _tetrisBoard.GetValue(x, y);
            Assert.AreEqual(expectedIsEmpty, Tile.IsNullOrEmpty(value));
        }

        [Test]
        [TestCase(0, 0, false)]
        [TestCase(1, 0, true)]
        public void GetValue_Valid_Input_int2(int x, int y, bool expectedIsEmpty)
        {
            var position = new int2(x, y);
            var value = _tetrisBoard.GetValue(position);
            Assert.AreEqual(expectedIsEmpty, Tile.IsNullOrEmpty(value));
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(TetrisBoard.Width, 0)]
        [TestCase(0, -1)]
        [TestCase(0, TetrisBoard.Height)]
        public void GetValue_Invalid_Input_Throws_Exception(int x, int y)
        {
            Assert.That(() => _tetrisBoard.GetValue(x, y),
                Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(0, 0, false)]
        [TestCase(0, 1, true)]
        public void SetValue_Valid_Input_XY(int x, int y, bool expectedIsEmpty)
        {
            var tile = expectedIsEmpty ? null : new Tile(_testSprite);
            _tetrisBoard.SetValue(x, y, tile);
            var actualValue = _tetrisBoard.GetValue(x, y);

            Assert.AreEqual(expectedIsEmpty, Tile.IsNullOrEmpty(actualValue));
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(TetrisBoard.Width, 0)]
        [TestCase(0, -1)]
        [TestCase(0, TetrisBoard.Height)]
        public void SetValue_Invalid_Input_Throws_Exception(int x, int y)
        {
            Assert.That(() => _tetrisBoard.SetValue(x, y, new Tile(_testSprite)),
                Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }
    }
}
