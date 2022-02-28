using UnityEngine;

namespace KevinSkyba.EGHG.Extensions
{
    public static class GameObjectExtensions
    {
        public static Transform RecursiveFind(this Transform self, string n)
        {
            Transform result = null;

            foreach (Transform child in self)
            {
                if (child.name == n)
                    result = child.transform;
                else
                    result = RecursiveFind(child, n);

                if (result != null) break;
            }

            return result;
        }
    }
}