using System.Collections.Generic;

//https://stackoverflow.com/questions/4515874/searching-for-a-fast-efficient-histogram-algorithm-with-pre-specified-bins
//linear transformation
namespace DataVisualization
{
    public class Histogram
    {
        private readonly int[] histogram;
        public IReadOnlyList<int> Frequency => histogram;
        private readonly ValueToIndexTransformation valueToIndexTransformation;

        public Histogram(IList<float> data, int bins)
        {
            valueToIndexTransformation = new ValueToIndexTransformation(data, bins);
            histogram = new int[bins];

            for (int i = 0; i < data.Count; i++)
            {
                int index = valueToIndexTransformation.ValueToIndex(data[i]);
                histogram[index]++;
            }
        }
    } 
}
