using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class AccelerationDataProvider : DataProvider
            {
                static public string ACCELERATION_HEAD_KEY = "headGaze.cartesian.acceleration";
                static public string ACCELERATION_EYE_KEY = "eyeGaze.cartesian.acceleration";
                static public string ACCELERATION_EYE_HEAD_KEY = "eyeGazeHeadGaze.cartesian.acceleration";

                /// <inheritdoc cref="EGHG.movingAverageWindow"/>
                private int movingAverageWindow;

                public AccelerationDataProvider(int movingAverageWindow)
                {
                    this.movingAverageWindow = movingAverageWindow;
                }

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    var dataPrecondition = dataSets.Count((x) => x.ContainsData(VelocityDataProvider.VELOCITY_EYE_KEY)) > movingAverageWindow + 1;
                    if (!dataPrecondition) return;

                    DataSet lastDataSet = dataSets[0];

                    var eulerEyeList = dataSets.Select((x) => x.GetData<Vector2>(VelocityDataProvider.VELOCITY_EYE_KEY));
                    var eyeAccelerations = eulerEyeList.Diff().Select(x => x / Time.fixedDeltaTime);

                    var eulerHeadList = dataSets.Select((x) => x.GetData<Vector2>(VelocityDataProvider.VELOCITY_HEAD_KEY));
                    var headAccelerations = Utility.Diff(eulerHeadList).Select(x => x / Time.fixedDeltaTime);

                    var eulerEyeHeadList = dataSets.Select((x) => x.GetData<Vector2>(VelocityDataProvider.VELOCITY_EYE_HEAD_KEY));
                    var headEyeAccelerations = Utility.Diff(eulerEyeHeadList).Select(x => x / Time.fixedDeltaTime);

                    lastDataSet.SetData(ACCELERATION_HEAD_KEY, Utility.MovingAverageVector2(movingAverageWindow, eyeAccelerations));
                    lastDataSet.SetData(ACCELERATION_EYE_KEY, Utility.MovingAverageVector2(movingAverageWindow, headAccelerations));
                    lastDataSet.SetData(ACCELERATION_EYE_HEAD_KEY, Utility.MovingAverageVector2(movingAverageWindow, headEyeAccelerations));
                }
            }
        }
    }
}