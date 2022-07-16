using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace DataVisualization.Editor.Tests
{
    public class GridTransformationTests
    {
        [Test]
        public void GlobalTransformationTest()
        {
            GridTransformation.Settings settings = new GridTransformation.Settings()
            {
                resolution = new Vector2Int(256, 256),
                minEdge = new Vector2(-10, -10),
                maxEdge = new Vector2(10, 10)
            };

            GridTransformation transformation = new GridTransformation(settings);
            transformation.RefreshMatrix();


            var comparer = new Vector3EqualityComparer(10e-6f);

            Vector3 global;

            transformation.RowColumnToGlobalPosition(0, 0, out global);
            Assert.That(global, Is.EqualTo(new Vector3(-10f, 0f, -10)).Using(comparer));

            transformation.RowColumnToGlobalPosition(255, 255, out global);
            Vector3 expected = new Vector3(10f, 0f, 10f) - transformation.Range;
            Assert.That(global, Is.EqualTo(expected).Using(comparer));
        }

        [Test]
        public void LocalTransformationTest()
        {
            GridTransformation.Settings settings = new GridTransformation.Settings()
            {
                resolution = new Vector2Int(256, 256),
                minEdge = new Vector2(-10, -10),
                maxEdge = new Vector2(10, 10)
            };

            GridTransformation transformation = new GridTransformation(settings);
            transformation.RefreshMatrix();

            int row;
            int column;

            transformation.GlobalPositionToRowColumn(new Vector3(-10f, 0f, 10), out row, out column);
            Assert.That(row, Is.EqualTo(255));
            Assert.That(column, Is.EqualTo(0));

            transformation.GlobalPositionToRowColumn(new Vector3(10f, 0f, -10), out row, out column);
            Assert.That(row, Is.EqualTo(0));
            Assert.That(column, Is.EqualTo(255));
        }
    }

}