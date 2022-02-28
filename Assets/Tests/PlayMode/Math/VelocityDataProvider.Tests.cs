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
                public class VelocityDataProviderTests
                {
                    private VelocityDataProvider velocityDataProvider;
                    private EulerDataProvider eulerDataProvider;
                    private DataSet[] dataSets;

                    [SetUp]
                    public void SetUp()
                    {
                        velocityDataProvider = new VelocityDataProvider(5);
                        eulerDataProvider = new EulerDataProvider();

                        dataSets = new DataSet[50];
                        for (int i = 0; i < 50; i++)
                        {
                            dataSets[i] = new DataSet();
                        }
                    }

                    [Test(Description = "Test whether this DataProvider does not work before the length of DataSets is atleast the size of the moving average window")]
                    public void Test_NotEnoughData()
                    {
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));

                        eulerDataProvider.OnDataSetAdded(dataSets.Take(1).ToArray());
                        velocityDataProvider.OnDataSetAdded(dataSets.Take(1).ToArray());

                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));

                        velocityDataProvider.OnDataSetAdded(dataSets.Take(4).ToArray());

                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));
                    }

                    [Test(Description = "Test the calculation of moving average and distances")]
                    [TestCase(1f, 12)]
                    [TestCase(5f, 25)]
                    [TestCase(15f, 33)]
                    public void Test_MovingAverageCalculation_1(float factor, int n)
                    {
                        // Prepare data
                        for (int i = 0; i < n; i++)
                        {
                            dataSets[i].SetData(EulerDataProvider.EULER_EYE_KEY, new Vector2(i * factor, 0));
                            dataSets[i].SetData(EulerDataProvider.EULER_HEAD_KEY, new Vector2(i * factor, 0));
                            dataSets[i].SetData(DistanceDataProvider.DISTANCE_DATA_KEY, i * factor);
                        }

                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsFalse(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));

                        velocityDataProvider.OnDataSetAdded(dataSets.Take(n).ToArray());

                        // First element should have data
                        Assert.IsTrue(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsTrue(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsTrue(dataSets[0].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));

                        // All other elements should not have data
                        Assert.IsFalse(dataSets[1].ContainsData(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                        Assert.IsFalse(dataSets[1].ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY));
                        Assert.IsFalse(dataSets[1].ContainsData(VelocityDataProvider.VELOCITY_HEAD_KEY));

                        // Average of Y should be 1, since eyeGaze is (1, 0, 0) and headGaze is (0, 0, 0)
                        Assert.AreEqual(factor / Time.fixedDeltaTime, dataSets[0].GetData<Vector2>(VelocityDataProvider.VELOCITY_EYE_KEY).x, 0.01f);
                        Assert.AreEqual(factor / Time.fixedDeltaTime, dataSets[0].GetData<Vector2>(VelocityDataProvider.VELOCITY_HEAD_KEY).x, 0.01f);
                    }
                }
            }
        }
    }
}