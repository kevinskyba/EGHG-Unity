using System.Collections.Generic;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        namespace Data
        {
            interface DataProvider
            {
                public void OnDataSetAdded(DataSet[] dataSets);
            }
        }
    }
}