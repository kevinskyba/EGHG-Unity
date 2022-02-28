using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            public class DataSet
            {
                private Quaternion headGaze;
                public Quaternion HeadGaze
                {
                    get { return headGaze; }
                    set { headGaze = value; }
                }

                private Quaternion eyeGaze;
                public Quaternion EyeGaze
                {
                    get { return eyeGaze; }
                    set { eyeGaze = value; }
                }

                private Vector3 headWorldPosition;
                public Vector3 HeadWorldPosition
                {
                    get { return headWorldPosition; }
                    set { headWorldPosition = value; }
                }

                private Dictionary<string, object> additionalData = new Dictionary<string, object>();

                
                public T GetData<T>(string name) {
                    if (additionalData.ContainsKey(name))
                        return (T)additionalData[name];
                    else
                        return default(T);
                }

                public void SetData<T>(string name, T data)
                {
                    additionalData[name] = data;
                }

                public bool ContainsData(string name) {
                    return additionalData.ContainsKey(name);
                }

                public void Reset()
                {
                    additionalData.Clear();
                }
            }
        }
    }
}