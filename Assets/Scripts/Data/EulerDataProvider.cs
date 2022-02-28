using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class EulerDataProvider : DataProvider
            {
                static public string EULER_HEAD_KEY = "headGaze.euler";
                static public string EULER_EYE_KEY = "eyeGaze.euler";

                public void OnDataSetAdded(DataSet[] dataSets)
                {
                    DataSet lastDataSet = dataSets[0];

                    Vector2 eulerEye = lastDataSet.EyeGaze.ToEuler2();
                    Vector2 eulerHead = lastDataSet.HeadGaze.ToEuler2();

                    // We dont want x to be the rotation AROUND x, but rather to be the rotation ON x. Same for y.
                    eulerEye.Set(eulerEye.y, eulerEye.x);
                    eulerHead.Set(eulerHead.y, eulerHead.x);

                    lastDataSet.SetData(EULER_EYE_KEY, eulerEye);
                    lastDataSet.SetData(EULER_HEAD_KEY, eulerHead);
                }
            }
        }
    }
}