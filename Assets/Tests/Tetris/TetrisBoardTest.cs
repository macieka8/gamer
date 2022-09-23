using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;
using gamer.tetris;
using Moq;

namespace gamer.tetris.Tests
{
    public class TetrisBoardTest
    {
        TetrisBoard _tetrisBoard;
        Sprite _testSprite;
        Tile _testTile;

        [SetUp]
        public void Setup()
        {
            _testSprite = Sprite.Create(new Texture2D(1, 1), Rect.zero, Vector2.zero);
            _testTile = new Tile(_testSprite);
            _tetrisBoard = new TetrisBoard();
            _tetrisBoard.SetValue(0, 0, _testTile);
            _tetrisBoard.SetValue(9, 0, _testTile);
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

        [Test]
        public void SetValue_Puzzle_ValidInput()
        {
            var testPuzzle = new Mock<IPuzzle>();
            var tiles = new Tile[]{_testTile, _testTile, _testTile, _testTile};
            var tilesOffset = new int2[]{new int2(0,0), new int2(1,0), new int2(0,1), new int2(1,1)};
            testPuzzle.Setup(p => p.Tiles).Returns(tiles);
            testPuzzle.Setup(p => p.GetTileOffset(0)).Returns(tilesOffset);
            testPuzzle.Setup(p => p.TilesCount).Returns(tiles.Length);

            var position = new int2(3, 3);
            var emptyBoard = new TetrisBoard();

            emptyBoard.SetValue(testPuzzle.Object, position, 0);
            for (int i = 0; i < testPuzzle.Object.TilesCount; i++)
            {
                var value = emptyBoard.GetValue(position + testPuzzle.Object.GetTileOffset(0)[i]);
                Assert.AreEqual(_testTile, value);
            }
        }

        [Test]
        [TestCase(5, 10, true)]
        [TestCase(0, 0, true)]
        [TestCase(TetrisBoard.Width - 1, 0, true)]
        [TestCase(0, TetrisBoard.Height - 1, true)]
        [TestCase(TetrisBoard.Width - 1, TetrisBoard.Height - 1, true)]
        [TestCase(-1, 10, false)]
        [TestCase(TetrisBoard.Width, 10, false)]
        [TestCase(5, -1, false)]
        [TestCase(5, TetrisBoard.Height, false)]
        public void IsPositionOnBoard_Cases(int x, int y, bool expectedValue)
        {
            var value = _tetrisBoard.IsPositionOnBoard(new int2(x, y));
            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        [TestCase(5, 5, true)]
        [TestCase(1, 1, true)]
        [TestCase(0, 1, false)]
        [TestCase(5, TetrisBoard.Height - 1, true)]
        [TestCase(TetrisBoard.Width - 1, 1, false)]
        public void FitsOnBoard_Empty_Board(int x, int y, bool expectedValue)
        {
            var offsets = new int2[] {
                new int2(0,0),
                new int2(1,0),
                new int2(-1,0),
                new int2(0,-1),
            };
            var value = _tetrisBoard.FitsOnBoard(offsets, new int2(x, y));
            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        [TestCase(5, 5, false)]
        [TestCase(4, 5, false)]
        [TestCase(6, 5, false)]
        [TestCase(5, 6, false)]
        [TestCase(5, 4, true)]
        public void FitsOnBoard_Collision_With_Tiles(int x, int y, bool expectedValue)
        {
            var testBoard = new TetrisBoard();
            testBoard.SetValue(5, 5, _testTile);

            var offsets = new int2[] {
                new int2(0,0),
                new int2(1,0),
                new int2(-1,0),
                new int2(0,-1),
            };

            var value = testBoard.FitsOnBoard(offsets, new int2(x, y));
            Assert.AreEqual(expectedValue, value);
        }
    }
}
