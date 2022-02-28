using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class DistanceDataProvider : DataProvider
            {
                static public string DIFFERENCE_DATA_KEY = "eyeGazeHeadGaze.difference";
                static public string DISTANCE_DATA_KEY = "eyeGazeHeadGaze.distances";

                /// <inheritdoc cref="EGHG.movingAverageWindow"/>
                private int movingAverageWindow;

                public DistanceDataProvider(int movingAverageWindow)
                {
                    this.movingAverageWindow = movingAverageWindow;
                }

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    if (dataSets.Length < movingAverageWindow) return;
                    DataSet lastDataSet = dataSets[0];

                    /**
                     * Calculation of eyeGazeHeadGaze.distance.x, eyeGazeHeadGaze.distance.y and eyeGazeHeadGaze.distance
                     */
                    var eulerEyeList = dataSets.Select((x) => x.GetData<Vector2>(EulerDataProvider.EULER_EYE_KEY));
                    var eulerHeadList = dataSets.Select((x) => x.GetData<Vector2>(EulerDataProvider.EULER_HEAD_KEY));

                    // Distance is equal to eulerEye values
                    var distanceMA = Utility.MovingAverageVector2(movingAverageWindow, eulerEyeList);
                    lastDataSet.SetData(DIFFERENCE_DATA_KEY, distanceMA);
                    lastDataSet.SetData(DISTANCE_DATA_KEY, Mathf.Sqrt(Mathf.Pow(distanceMA.x, 2) + Mathf.Pow(distanceMA.y, 2)));
                }
            }
        }
    }
}