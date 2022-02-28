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
                public class FixationDataProviderTests
                {
                    private FixationDataProvider fixationDataProvider;
                    private DataSet[] dataSets;

                    [SetUp]
                    public void SetUp()
                    {
                        fixationDataProvider = new FixationDataProvider(20, 5);

                        dataSets = new DataSet[50];
                        for (int i = 0; i < 50; i++)
                        {
                            dataSets[i] = new DataSet();
                        }
                    }

                    [Test(Description = "Test whether this DataProvider handles not enough data")]
                    public void Test_NotEnoughData()
                    {
                        return;
                    }
                }
            }
        }
    }
}