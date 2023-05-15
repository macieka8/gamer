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
    public class PuzzleMoverTest
    {
        TetrisBoard _emptyBoard;
        PuzzleMover _puzzleMover;
        Mock<IPuzzle> _testPuzzle;
        Sprite _testSprite;
        Tile _testTile;

        [SetUp]
        public void Setup()
        {
            _testSprite = Sprite.Create(new Texture2D(1, 1), Rect.zero, Vector2.zero);
            _testTile = new Tile(_testSprite);
            _emptyBoard = new TetrisBoard();

            _testPuzzle = new Mock<IPuzzle>();
            var tiles = new Tile[]{_testTile, _testTile, _testTile, _testTile};
            var tilesOffset = new int2[]{new int2(0,0), new int2(1,0), new int2(0,1), new int2(1,1)};
            _testPuzzle.Setup(p => p.Tiles).Returns(tiles);
            _testPuzzle.Setup(p => p.GetTileOffset(0)).Returns(tilesOffset);
            _testPuzzle.Setup(p => p.TilesCount).Returns(tiles.Length);
        }

        [Test]
        public void CanMoveLeft_Left_Wall_Collision()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(0, 0));

            var value = _puzzleMover.CanMoveLeft();
            Assert.AreEqual(false, value);
        }

        [Test]
        public void CanMoveLeft_Valid_Move()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(5, 0));

            var value = _puzzleMover.CanMoveLeft();
            Assert.AreEqual(true, value);
        }

        [Test]
        public void CanMoveLeft_Tile_Collision()
        {
            var testBoard = new TetrisBoard();
            testBoard.SetValue(0, 1, new Tile(_testSprite));

            _puzzleMover = new PuzzleMover(testBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(1, 0));

            var value = _puzzleMover.CanMoveLeft();
            Assert.AreEqual(false, value);
        }

        [Test]
        public void CanMoveRight_Right_Wall_Collision()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(8, 0));

            var value = _puzzleMover.CanMoveRight();
            Assert.AreEqual(false, value);
        }

        [Test]
        public void CanMoveRight_Valid_Move()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(5, 0));

            var value = _puzzleMover.CanMoveRight();
            Assert.AreEqual(true, value);
        }

        [Test]
        public void CanMoveRight_Tile_Collision()
        {
            var testBoard = new TetrisBoard();
            testBoard.SetValue(6, 1, new Tile(_testSprite));

            _puzzleMover = new PuzzleMover(testBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(4, 0));

            var value = _puzzleMover.CanMoveRight();
            Assert.AreEqual(false, value);
        }

        [Test]
        public void CanMoveDown_Ground_Collision()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(0, 18));

            var value = _puzzleMover.CanMoveDown();
            Assert.AreEqual(false, value);
        }

        [Test]
        public void CanMoveDown_Valid_Move()
        {
            _puzzleMover = new PuzzleMover(_emptyBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(5, 0));

            var value = _puzzleMover.CanMoveDown();
            Assert.AreEqual(true, value);
        }

        [Test]
        public void CanMoveDown_Tile_Collision()
        {
            var testBoard = new TetrisBoard();
            testBoard.SetValue(6, 5, new Tile(_testSprite));

            _puzzleMover = new PuzzleMover(testBoard);
            _puzzleMover.SetActivePuzzle(_testPuzzle.Object, new int2(5, 3));

            var value = _puzzleMover.CanMoveDown();
            Assert.AreEqual(false, value);
        }
    }
}
