using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class VelocityDataProvider : DataProvider
            {
                static public string VELOCITY_HEAD_KEY = "headGaze.cartesian.velocity";
                static public string VELOCITY_EYE_KEY = "eyeGaze.cartesian.velocity";
                static public string VELOCITY_EYE_HEAD_KEY = "eyeGazeHeadGaze.cartesian.velocity";

                /// <inheritdoc cref="EGHG.movingAverageWindow"/>
                private int movingAverageWindow;

                public VelocityDataProvider(int movingAverageWindow)
                {
                    this.movingAverageWindow = movingAverageWindow;
                }

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    var dataPrecondition = dataSets.Count((x) => x.ContainsData(EulerDataProvider.EULER_EYE_KEY)) > movingAverageWindow + 1;
                    if (!dataPrecondition) return;

                    DataSet lastDataSet = dataSets[0];

                    var eulerEyeList = dataSets.Select((x) => x.GetData<Vector2>(EulerDataProvider.EULER_EYE_KEY));
                    var eyeVelocities = eulerEyeList.Diff().Select(x => x / Time.fixedDeltaTime).ToList();

                    var eulerHeadList = dataSets.Select((x) => x.GetData<Vector2>(EulerDataProvider.EULER_HEAD_KEY));
                    var headVelocities = Utility.Diff(eulerHeadList).Select(x => x / Time.fixedDeltaTime);

                    var eulerEyeHeadList = dataSets.Select((x) => x.GetData<Vector2>(DistanceDataProvider.DIFFERENCE_DATA_KEY));
                    var headEyeVelocities = Utility.Diff(eulerEyeHeadList).Select(x => x / Time.fixedDeltaTime);

                    lastDataSet.SetData(VELOCITY_HEAD_KEY, Utility.MovingAverageVector2(movingAverageWindow, eyeVelocities));
                    lastDataSet.SetData(VELOCITY_EYE_KEY, Utility.MovingAverageVector2(movingAverageWindow, headVelocities));
                    lastDataSet.SetData(VELOCITY_EYE_HEAD_KEY, Utility.MovingAverageVector2(movingAverageWindow, headEyeVelocities));
                }
            }
        }
    }
}