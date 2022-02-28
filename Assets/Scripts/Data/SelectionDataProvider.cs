using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class SelectionDataProvider : DataProvider
            {
                static public string SELECTION_KEY = "eyeGazeHeadGaze.selection";
                static public string X_ACCELERATION_MEANS_KEY = "eyeGazeHeadGaze.distance.x_acceleration.means";
                static public string X_ACCELERATION_QUANTILES_KEY = "eyeGazeHeadGaze.distance.x_acceleration.quantiles";

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionLookback"/>
                private int selectionLookback;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionFixation"/>
                private float selectionFixation;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionQuantile"/>
                private float selectionQuantile;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionTolerance"/>
                private float selectionTolerance;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionTargetAcceleration"/>
                private float selectionTargetAcceleration;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionMaxAcceleration"/>
                private float selectionMaxAcceleration;

                /// <inheritdoc cref="EGHG.EGHGParameters.selectionDistance"/>
                private float selectionDistance;

                public SelectionDataProvider(int selectionLookback, float selectionQuantile, 
                    float selectionTolerance, float selectionTargetAcceleration, float selectionMaxAcceleration,
                    float selectionFixation, float selectionDistance)
                {
                    this.selectionLookback = selectionLookback;
                    this.selectionQuantile = selectionQuantile;
                    this.selectionTolerance = selectionTolerance;
                    this.selectionTargetAcceleration = selectionTargetAcceleration;
                    this.selectionMaxAcceleration = selectionMaxAcceleration;
                    this.selectionFixation = selectionFixation;
                    this.selectionDistance = selectionDistance;
                }

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    var dataSetsCount = dataSets.Count((x) =>
                        x.ContainsData(FixationDataProvider.FIXATION_KEY) && x.ContainsData(AccelerationDataProvider.ACCELERATION_EYE_HEAD_KEY));
                    var dataPrecondition = dataSetsCount >= selectionLookback;
                    if (!dataPrecondition) return;

                    DataSet lastDataSet = dataSets[0];

                    var x_accelerations = dataSets.Take(selectionLookback).Select((x) => x.GetData<Vector2>(AccelerationDataProvider.ACCELERATION_EYE_HEAD_KEY).x);
                    var x_velocities = dataSets.Take(selectionLookback).Select((x) => x.GetData<Vector2>(VelocityDataProvider.VELOCITY_HEAD_KEY).x);
                    var fixations = dataSets.Take(selectionLookback).Select((x) => x.GetData<float>(FixationDataProvider.FIXATION_KEY));
                    var distances = dataSets.Take(selectionLookback).Select((x) => x.GetData<float>(DistanceDataProvider.DISTANCE_DATA_KEY));

                    var accelerationMean = x_accelerations.Sum() / x_accelerations.Count();
                    var quantile = x_accelerations.Percentile(selectionQuantile);
                    var max = x_accelerations.Max();
                    var min = x_accelerations.Min();

                    var maxFixation = fixations.Max();
                    var maxDistance = distances.Max();
                    var averageXVelocity = x_velocities.Average();

                    var selection = true;

                    if (Mathf.Abs(accelerationMean) > selectionTolerance)
                        selection = false;

                    if (max < selectionTargetAcceleration - selectionTolerance)
                        selection = false;

                    if (min > -selectionTargetAcceleration + selectionTolerance * 2)
                        selection = false;

                    if (max > selectionMaxAcceleration)
                        selection = false;

                    if (min < -selectionMaxAcceleration)
                        selection = false;

                    if (maxFixation < selectionFixation)
                        selection = false;

                    if (maxDistance < selectionDistance)
                        selection = false;

                    if (averageXVelocity < 0)
                        selection = false;

                    lastDataSet.SetData(SELECTION_KEY, selection);
                    lastDataSet.SetData(X_ACCELERATION_MEANS_KEY, accelerationMean);
                    lastDataSet.SetData(X_ACCELERATION_QUANTILES_KEY, quantile);
                }

            }
        }
    }
}