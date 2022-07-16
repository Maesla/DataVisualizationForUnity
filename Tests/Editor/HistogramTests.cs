using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace DataVisualization.Editor.Tests
{
    [TestFixture]
    public class HistogramTests
    {
        [Test]
        [TestCase(new[] { 1, 1, 4 }, new float[] { -7, 0, 3, 3, 8, 3 }, 3)]
        [TestCase(new[] { 7, 5, 3 }, new float[] { 22, 87, 5, 43, 56, 73, 55, 54, 11, 20, 51, 5, 79, 31, 27 }, 3)]
        [TestCase(new[] {4,3,3,2,3}, new float[]{22, 87, 5, 43, 56, 73, 55, 54, 11, 20, 51, 5, 79, 31, 27}, 5)]
        public void HistogramTest(int [] expectedFrequencys, float [] data, int bins)
        {
            var histogram = new Histogram(data, bins);

            CollectionAssert.AreEquivalent(expectedFrequencys, histogram.Frequency);
        }

        public static IEnumerable<TestCaseData> Histogram2dCases()
        {
            var x = new float[] { -7, 0, 3, 3, 8, 3 };
            var y = new float[] { -5, 6, 2, 2, 4, 6 };
            var expected = new float[0, 0];

            expected = new float[,] { { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 2, 0 }, { 0, 1, 1, 1 } };
            yield return new TestCaseData(expected, x, y, 4, 4, false);

            expected = new float[,] { { 1, 0, 0, 0 }, { 0, 0, 2, 0 }, { 0, 1, 1, 1 } };
            yield return new TestCaseData(expected, x,y,4,3,false);
            
            expected = new float[,]
            { 
                { 0.01616f, 0, 0, 0 }, 
                { 0, 0, 0, 0 },
                { 0, 0, 0.0323f, 0 }, 
                { 0, 0.01616f, 0.01616f, 0.01616f } 
            };
            yield return new TestCaseData(expected, x, y, 4, 4, true);

            expected = new float[,]
            {
                { 0.0121f, 0, 0, 0 }, 
                { 0, 0, 0.0242f, 0 }, 
                { 0, 0.0121f, 0.0121f, 0.0121f } 
            };
            yield return new TestCaseData(expected, x, y, 4, 3, true);
        }

        [Test]
        [TestCaseSource(nameof(Histogram2dCases))]
        public void Histogram2dTest(float[,] expectedFrequencys, float[] x, float[] y, int xBins, int yBins, bool density)
        {

            using (var hist = new Histogram2d(x, y, xBins, yBins, density))
            {
                Assert.That(hist.Frequency, Is.EqualTo(expectedFrequencys).Using<object>(GridComparasion));
            }
        }

        private int GridComparasion(object multimensionalArrayObject, object gridObject)
        {
            var grid = (IGrid)gridObject;
            var multidimensionalArray = (float[,])multimensionalArrayObject;

            if (grid == null || multidimensionalArray == null)
            {
                return -1;
            }

            if (grid.RowCount != multidimensionalArray.GetLength(0) || grid.ColumnCount != multidimensionalArray.GetLength(1))
            {
                return -1;
            }


            for (int i = 0; i < grid.RowCount; i++)
            {
                for (int j = 0; j < grid.ColumnCount; j++)
                {
                    var actual = grid[i, j];
                    var expected = multidimensionalArray[i, j];
                    var errorMessage = FormattableString.Invariant($"Expected {expected} but was {actual} at [{i},{j}]");
                    Assert.That(actual, Is.EqualTo(expected).Within(1e-2), errorMessage);
                }
            }

            return 0;
        }
    }

}