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
                public class EGHGTests
                {
                    private EGHG eghg;
                    private Queue<DataSet> dataSet_A;
                    private Queue<DataSet> dataSet_B;

                    private EGHG.EGHGParameters defaultParameters;

                    [SetUp]
                    public void SetUp()
                    {
                        defaultParameters = new EGHG.EGHGParameters()
                        {
                            movingAverageWindow = 10,

                            fixationLookback = 15,
                            fixationRange = 5,

                            selectionFixation = 0.7f,
                            selectionTolerance = 150,
                            selectionTargetAcceleration = 600,
                            selectionLookback = 40,
                            selectionQuantile = 0.5f
                        };
                        eghg = new EGHG(50, defaultParameters);


                        // dataSet_A will do an eye-gaze X movement of sin(x/50 * pi)
                        dataSet_A = new Queue<DataSet>();
                        for (int i = 0; i < 100; i++)
                        {
                            DataSet dataSet = new DataSet();
                            dataSet.EyeGaze = Quaternion.Euler(new Vector3(Mathf.Sin(i / 50 * Mathf.PI * Mathf.Deg2Rad), 0, 0));
                            dataSet_A.Enqueue(dataSet);
                        }


                        // dataSet_A will do an eye-gaze X movement of i
                        dataSet_B = new Queue<DataSet>();
                        for (int i = 0; i < 100; i++)
                        {
                            DataSet dataSet = new DataSet();
                            dataSet.EyeGaze = Quaternion.Euler(new Vector3(i, 0, 0));
                            dataSet_B.Enqueue(dataSet);
                        }
                    }

                    [Test(Description = "Just an example")]
                    public void Test_Example()
                    {
                        Assert.AreEqual(0, eghg.DataSetsLength);

                        var next = dataSet_A.Dequeue();
                        eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);

                        Assert.AreEqual(1, eghg.DataSetsLength);

                        Assert.IsFalse(eghg.DataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(eghg.DataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        next = dataSet_A.Dequeue();
                        eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                        Assert.AreEqual(2, eghg.DataSetsLength);

                        Assert.IsFalse(eghg.DataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsFalse(eghg.DataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));

                        for (int i = 3; i < 16; i++)
                        {
                            next = dataSet_A.Dequeue();
                            eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                            Assert.AreEqual(i, eghg.DataSetsLength);
                        }

                        Assert.IsTrue(eghg.DataSets[0].ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                        Assert.IsTrue(eghg.DataSets[0].ContainsData(DistanceDataProvider.DIFFERENCE_DATA_KEY));
                    }

                    [Test(Description = "Test whether the DataSet is properly being moved backwards, so that index 0 becomes index 1, index 1 becomes 2 and so on...")]
                    [TestCase(5, 1)]
                    [TestCase(10, 3)]
                    [TestCase(25, 10)]
                    public void Test_Queue_Behaviour_001(int n, int movingAverage)
                    {
                        eghg = new EGHG(n, defaultParameters);

                        DataSet previous = null;
                        for (int i = 0; i < n; i++)
                        {
                            var next = dataSet_B.Dequeue();
                            eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                            Assert.AreEqual(i+1, eghg.DataSetsLength);

                            if (previous != null && i > movingAverage)
                            {
                                Assert.AreEqual(previous.GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), eghg.DataSets[1].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), 0.001f);
                            }
                            previous = eghg.DataSets[0];
                        }
                    }

                    [Test(Description = "Test whether the DataSet is properly being moved backwards, so that index 0 becomes index 1, index 1 becomes 2 and so on...")]
                    [TestCase(5, 1)]
                    [TestCase(10, 3)]
                    [TestCase(25, 10)]
                    public void Test_Queue_Behaviour_002(int n, int movingAverage)
                    {
                        eghg = new EGHG(n, defaultParameters);

                        Assert.AreEqual(0, eghg.DataSetsLength);

                        var next = dataSet_B.Dequeue();
                        eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                        Assert.AreEqual(1, eghg.DataSetsLength);

                        next = dataSet_B.Dequeue();
                        var last = eghg.DataSets[0];
                        eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                        Assert.AreEqual(2, eghg.DataSetsLength);

                        Assert.AreEqual(last.GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), eghg.DataSets[1].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), 0.001f);

                        next = dataSet_B.Dequeue();
                        last = eghg.DataSets[0];
                        eghg.AddDataSet(next.HeadGaze, next.EyeGaze, next.HeadWorldPosition);
                        Assert.AreEqual(3, eghg.DataSetsLength);

                        Assert.AreEqual(last.GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), eghg.DataSets[1].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY), 0.001f);
                    }
                }
            }
        }
    }
}