using NUnit.Framework;

namespace DataVisualization.Editor.Tests
{
    [TestFixture]
    public class KernelTests
    {
        [TestCase(0,0, 0.3989f)]
        [TestCase(1,2, 0.007306882745281f)]
        [TestCase(-1,2, 0.007306882745281f)]
        [TestCase(1,-2, 0.007306882745281f)]
        [TestCase(10,0, 0f)]
        public void KernelTest(int rowDiff, int columnDiff, float expectedResult)
        {
            using (Kernel k = new Kernel(0.2f, 0.4f, 0.2f, 3))
            {
                //float distance = sqrt((rowDiff*yRange)^2 + (columnDiff*xRange)^2)
                //float result = gaussian(distance/bandwith)
                Assert.That(k.Calculate(rowDiff, columnDiff), Is.EqualTo(expectedResult).Within(1e-3f));
            }
        }
    } 
}
