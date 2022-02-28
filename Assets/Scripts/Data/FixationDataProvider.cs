using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class FixationDataProvider : DataProvider
            {
                static public string FIXATION_KEY = "eyeGazeHeadGaze.fixation";

                /// <inheritdoc cref="EGHG.EGHGParameters.fixationLookback"/>
                private int fixationLookback;

                /// <inheritdoc cref="EGHG.EGHGParameters.fixationRange"/>
                private float fixationRange;

                public FixationDataProvider(int fixationLookback, float fixationRange)
                {
                    this.fixationLookback = fixationLookback;
                    this.fixationRange = fixationRange;
                }

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    var dataSetsCount = dataSets.Count((x) => x.ContainsData(DistanceDataProvider.DISTANCE_DATA_KEY));
                    var dataPrecondition = dataSetsCount >= fixationLookback;
                    if (!dataPrecondition) return;

                    DataSet lastDataSet = dataSets[0];

                    float fixation = 0f;
                    for (int i = 0; i < Mathf.Min(dataSetsCount, fixationLookback); i++)
                    {
                        fixation += Mathf.Max(0, (Mathf.Abs(dataSets[i].GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY)) - fixationRange) * Decay(i));
                    }

                    fixation /= Mathf.Min(dataSetsCount, fixationLookback);

                    if (fixation > 0)
                    {
                        fixation = Mathf.Max(0, Mathf.Min(1, 1 / fixation));
                    } else
                    {
                        fixation = 1f;
                    }

                    lastDataSet.SetData(FIXATION_KEY, fixation);
                }

                private float Decay(float x)
                {
                    return 1 / Mathf.Sqrt(0.25f * x + 1);
                }
            }
        }
    }
}