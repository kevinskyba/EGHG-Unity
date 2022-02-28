using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KevinSkyba.EGHG.Data;

namespace KevinSkyba.EGHG
{
    /// <summary>
    /// This class manages a data processing pipeline.
    /// Each time <see cref="AddDataSet(Quaternion, Quaternion, Vector3)"/> is called, a pipeline is running
    /// which is appending data to the DataSets in <see cref="DataSets"/>.
    /// <see cref="DataSets"/> are managed as a queue. There will never be more than <see cref="dataSetCount"/>
    /// DataSets, where the current amount of DataSets is stored in <see cref="DataSetsLength"/>.
    /// Each time a new <see cref="DataSet"/> is added, it is put in front of the DataSet array. All other
    /// items are moved one index behind. The behaviour is equal to that of a queue, except that here you can
    /// actually access items by index.
    /// </summary>
    public class EGHG
    {
        /// <summary>
        /// The queue that is containing the last <see cref="dataSetCount"/> number of DataSets.
        /// </summary>
        private DataSet[] dataSets;
        public DataSet[] DataSets { get { return dataSets; } }

        public int DataSetsLength { get { return dataSetLength; } }

        /// <summary>
        /// The number of <see cref="DataSet"/>s in <see cref="dataSets"/>.
        /// </summary>
        private int dataSetCount;

        private int dataSetLength;

        public struct EGHGParameters
        {
            /// <summary>
            /// Moving Average Window used by DataProviders to smooth out data where applicable.
            /// </summary>
            public int movingAverageWindow;

            /// <summary>
            /// When determining the fixation, how many frames to look back for calculating the fixation.
            /// </summary>
            public int fixationLookback;

            /// <summary>
            /// The base range in which fixation will always be 100%.
            /// </summary>
            public float fixationRange;

            /// <summary>
            /// When determining selection, how many frames to look back for calculating the selection.
            /// </summary>
            public int selectionLookback;

            /// <summary>
            /// What minimum fixation is required inside of the selectionLookback window
            /// </summary>
            public float selectionFixation;

            /// <summary>
            /// Not currently used.
            /// </summary>
            public float selectionQuantile;

            /// <summary>
            /// What tolerance is applied to data (e.g. <see cref="selectionTargetAcceleration"/>) withing selectionLookback window.
            /// </summary>
            public float selectionTolerance;

            /// <summary>
            /// What acceleration is required to trigger a selection.
            /// </summary>
            public float selectionTargetAcceleration;

            /// <summary>
            /// What acceleration is causing the selection to fail automatically.
            /// </summary>
            public float selectionMaxAcceleration;

            public float selectionDistance;

        }
        private EGHGParameters parameters;
        public EGHGParameters Parameters => parameters;

        private List<DataProvider> dataProviders;
        private EulerDataProvider eulerDataProvider;
        private DistanceDataProvider distanceDataProvider;
        private VelocityDataProvider velocityDataProvider;
        private AccelerationDataProvider accelerationDataProvider;
        private FixationDataProvider fixationDataProvider;
        private SelectionDataProvider selectionDataProvider;

        public EGHG(int dataSetCount, EGHGParameters parameters)
        {
            this.dataSetCount = dataSetCount;
            this.parameters = parameters;

            dataProviders = new List<DataProvider>();

            eulerDataProvider = new EulerDataProvider();
            dataProviders.Add(eulerDataProvider);

            distanceDataProvider = new DistanceDataProvider(parameters.movingAverageWindow);
            dataProviders.Add(distanceDataProvider);

            velocityDataProvider = new VelocityDataProvider(parameters.movingAverageWindow);
            dataProviders.Add(velocityDataProvider);

            accelerationDataProvider = new AccelerationDataProvider(parameters.movingAverageWindow);
            dataProviders.Add(accelerationDataProvider);

            fixationDataProvider = new FixationDataProvider(parameters.fixationLookback, parameters.fixationRange);
            dataProviders.Add(fixationDataProvider);

            selectionDataProvider = new SelectionDataProvider(
                parameters.selectionLookback,
                parameters.selectionQuantile,
                parameters.selectionTolerance,
                parameters.selectionTargetAcceleration,
                parameters.selectionMaxAcceleration,
                parameters.selectionFixation,
                parameters.selectionDistance);
            dataProviders.Add(selectionDataProvider);

            dataSets = new DataSet[dataSetCount];
            for (int i = 0; i < dataSetCount; i++)
            {
                dataSets[i] = new DataSet();
            }
            dataSetLength = 0;
        }

        public void AddDataSet(Quaternion headGaze, Quaternion eyeGaze, Vector3 headWorldPosition)
        {
            DataSet dataSet = dataSets[dataSetCount - 1];
            dataSet.Reset();

            // Shift queue by 1
            Array.Copy(dataSets, 0, dataSets, 1, dataSetCount - 1);

            // Create new entry with pooling
            dataSet.HeadGaze = headGaze;
            dataSet.EyeGaze = eyeGaze;
            dataSet.HeadWorldPosition = headWorldPosition;

            dataSets[0] = dataSet;

            if (dataSetLength < dataSetCount)
                dataSetLength++;

            OnDataSetAdded();
        }

        public void Reset()
        {
            dataSetLength = 0;
        }

        private void OnDataSetAdded()
        {
            dataProviders.ForEach(p => p.OnDataSetAdded(dataSets.Take(dataSetLength).ToArray()));
        }
    }
}