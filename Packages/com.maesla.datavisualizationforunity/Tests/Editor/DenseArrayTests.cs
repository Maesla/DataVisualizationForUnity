using NUnit.Framework;

namespace DataVisualization.Editor.Tests
{
    public class DenseArrayTests
    {
        [Test]
        [TestCase(new[] { 5 }, new[] { 2 }, 2)]
        [TestCase(new[] { 5, 4 }, new[] { 2, 3 }, 11)]
        [TestCase(new[] { 5, 4 }, new[] { 3, 3 }, 15)]
        [TestCase(new[] { 5, 4 }, new[] { 0, 2 }, 2)]
        [TestCase(new[] { 5, 4 }, new[] { 2, 0 }, 8)]
        [TestCase(new[] { 5, 4, 3 }, new[] { 2, 3, 1 }, 34)]
        [TestCase(new[] { 5, 4, 3 }, new[] { 2, 3, 0 }, 33)]
        public void IndexTest(int[] size, int[] indices, int expectedIndex)
        {
            DenseArray<int> denseArray = new DenseArray<int>(size);
            Assert.That(denseArray.CalculateIndex(indices), Is.EqualTo(expectedIndex));
        }

        [Test]
        public void MultiDimensionalArrayEquivalentTest()
        {
            int[] size = { 20, 30, 10 };

            float[,,] m = new float[size[0], size[1], size[2]];
            using (DenseArray<float> denseArray = new DenseArray<float>(size))
            {

                for (int i = 0; i < size[0]; i++)
                {
                    for (int j = 0; j < size[1]; j++)
                    {
                        for (int k = 0; k < size[2]; k++)
                        {
                            float value = 5 * i - k + 2 * j; // random value;
                            m[i, j, k] = value;
                            denseArray[i, j, k] = value;
                        }
                    }
                }

                for (int i = 0; i < size[0]; i++)
                {
                    for (int j = 0; j < size[1]; j++)
                    {
                        for (int k = 0; k < size[2]; k++)
                        {
                            float f1 = m[i, j, k];
                            float f2 = denseArray[i, j, k];


                            Assert.That(f1, Is.EqualTo(f2));
                        }
                    }
                }
            }
        }
    } 
}
