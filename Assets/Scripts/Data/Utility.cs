using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            static class Utility
            {

                /// <summary>
                /// There is a ToEuler function already, but this one is equal to the one used in the jupyter notebook.
                /// </summary>
                static public Vector3 ToEuler2(this Quaternion quaternion)
                {
                    var y = quaternion.y;
                    var w = quaternion.w;
                    var x = quaternion.x;
                    var z = quaternion.z;

                    var ysqr = y * y;

                    var t0 = 2.0f * (w * x + y * z);
                    var t1 = 1.0f - 2.0f * (x * x + ysqr);
                    var X = Mathf.Atan2(t0, t1) * Mathf.Rad2Deg;

                    var t2 = 2.0f * (w * y - z * x);
                    t2 = Mathf.Min(1.0f, t2);
                    t2 = Mathf.Max(-1.0f, t2);

                    var Y = Mathf.Asin(t2) * Mathf.Rad2Deg;

                    var t3 = 2.0f * (w * z + x * y);
                    var t4 = 1.0f - 2.0f * (ysqr + z * z);
                    var Z = Mathf.Atan2(t3, t4) * Mathf.Rad2Deg;

                    return new Vector3(X, Y, Z);
                }

                static public Vector2 MovingAverageVector2(int window, IEnumerable<Vector2> data)
                {
                    Vector2 mean = Vector2.zero;
                    for (int k = 0; k < Mathf.Min(window, data.Count()); k++)
                    {
                        mean += data.ElementAt(k);
                    }
                    mean /= window;

                    return mean;
                }


                static public IEnumerable<Vector2> Diff(List<Vector2> data)
                {
                    return data.Zip(data.Skip(1), (a, b) => {
                        Vector2 _a = a;
                        Vector2 _b = b;
                        return (Vector2)(_b - _a);
                    });
                }

                static public IEnumerable<Vector3> Diff(List<Vector3> data) 
                {
                    return data.Zip(data.Skip(1), (a, b) => {
                        Vector3 _a = a;
                        Vector3 _b = b;
                        return (Vector3)(_b - _a);
                    });
                }

             
                static public IEnumerable<Vector2> Diff(this IEnumerable<Vector2> source) 
                {
                    return source.Zip(source.Skip(1), (a, b) => {
                        Vector2 _a = a;
                        Vector2 _b = b;
                        return (Vector2)(_b -_a);
                    });
                }

                static public IEnumerable<Vector3> Diff(this IEnumerable<Vector3> source)
                {
                    return source.Zip(source.Skip(1), (a, b) => {
                        Vector3 _a = a;
                        Vector3 _b = b;
                        return (Vector3)(_b -_a);
                    });
                }

              

                static public double Percentile(this IEnumerable<float> source, double excelPercentile)
                {
                    var sequence = source.OrderBy((x) => x);
                    int N = sequence.Count();
                    double n = (N - 1) * excelPercentile + 1;
                    // Another method: double n = (N + 1) * excelPercentile;
                    if (n == 1d) return sequence.ElementAt(0);
                    else if (n == N) return sequence.ElementAt(N - 1);
                    else
                    {
                        int k = (int)n;
                        double d = n - k;
                        return sequence.ElementAt(k - 1) + d * (sequence.ElementAt(k) - sequence.ElementAt(k - 1));
                    }
                }
            }
        }
    }
}