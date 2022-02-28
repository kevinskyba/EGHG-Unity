using KevinSkyba.Pandas;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Pandas
        {
            namespace Tests
            {
                public class PandasConnectorTests
                {
                    private PandasConnector pandas;

                    [SetUp]
                    public void SetUp()
                    {
                        pandas = new PandasConnector(new PandasConnector.Settings("127.0.0.1", 9090), new Dictionary<string, object>());
                    }
                }
            }
        }
    }
}