using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            namespace Tests
            {
                public class DistanceDataProviderTests
                {
                    private DistanceDataProvider distanceDataProvider;
                    private EulerDataProvider eulerDataProvider;
                    private DataSet[] dataSets;

                    [SetUp]
                    public void SetUp()
                    {
                        distanceDataProvider = new DistanceDataProvider(5);
                        eulerDataProvider = new EulerDataProvider();

                        dataSets = new DataSet[20];
                        for (int i = 0; i < 20; i++)
                        {
                            dataSets[i] = new DataSet();
                        }
                    }

                    [Test(Description = "Test whether this DataProvider does not work before the length of DataSets is atleast the size of the moving average window")]
                    public void Test_NotEnoughData()
                    {
                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        eulerDataProvider.OnDataSetAdded(dataSets.Take(1).ToArray());
                        distanceDataProvider.OnDataSetAdded(dataSets.Take(1).ToArray());

                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        distanceDataProvider.OnDataSetAdded(dataSets.Take(4).ToArray());

                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));
                    }

                    [Test(Description = "Test the calculation of moving average and distances")]
                    [TestCase(1f, 5, 45f)]
                    [TestCase(5f, 7, 78.690068f)]
                    [TestCase(15f, 11, 86.18592f)]
                    public void Test_MovingAverageCalculation_1(float x, int n, float result)
                    {
                        // Prepare data
                        for (int i = 0; i < n; i++)
                        {
                            dataSets[i].HeadGaze = Quaternion.LookRotation(Vector3.forward, Vector3.up).normalized;
                            dataSets[i].EyeGaze =
                                Quaternion.LookRotation((new Vector3(x, 0, 1)).normalized, Vector3.up).normalized
                                *
                                Quaternion.Inverse(dataSets[i].HeadGaze);
                            eulerDataProvider.OnDataSetAdded(new[] { dataSets[i] });
                        }

                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        distanceDataProvider.OnDataSetAdded(dataSets.Take(n).ToArray());

                        // First element should have data
                        Assert.IsTrue(dataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsTrue(dataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        // All other elements should not have data
                        Assert.IsFalse(dataSets[1].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(dataSets[1].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        // Average of Y should be 1, since eyeGaze is (1, 0, 0) and headGaze is (0, 0, 0)
                        Assert.AreEqual(result, dataSets[0].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), 0.01f);
                    }


                    [Test(Description = "Test the calculation of moving average and distances")]
                    [TestCase(5, 5)]
                    [TestCase(8, 7)]
                    [TestCase(15, 11)]
                    public void Test_MovingAverageCalculation_2(int n, int movingAverage)
                    {
                        distanceDataProvider = new DistanceDataProvider(movingAverage);
                        List<DataSet> dataSets = new List<DataSet>();

                        // Prepare data
                        for (int i = 0; i < n; i++)
                        {
                            DataSet dataSet = new DataSet();
                            dataSet.HeadGaze = Quaternion.LookRotation(Vector3.forward, Vector3.up).normalized;
                            dataSet.EyeGaze =
                                Quaternion.LookRotation((new Vector3(i, 0, 1)).normalized, Vector3.up).normalized
                                *
                                Quaternion.Inverse(dataSet.HeadGaze);
                            eulerDataProvider.OnDataSetAdded(new[] { dataSet });
                            dataSets.Add(dataSet);
                            distanceDataProvider.OnDataSetAdded(dataSets.Reverse<DataSet>().ToArray());

                            if (i >= movingAverage - 1)
                            {
                                // The angle for each j is equal to sin^-1(j / sqrt(j^2 + 1)) 
                                float expected = 0;
                                for (int j = i; j > i - movingAverage; j--)
                                {
                                    expected += Mathf.Asin((j / Mathf.Sqrt(j * j + 1))) * Mathf.Rad2Deg;
                                }
                                expected /= movingAverage;
                                Assert.AreEqual(expected, dataSets.Reverse<DataSet>().ToArray()[0].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), 0.01f);
                            }
                        }
                    }
                }
            }
        }
    }
}