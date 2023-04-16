using CleverCrow.Fluid.StatsSystem;
using UnityEngine;

namespace Battle.Data.Stats
{
    public static class StatRecordExtension
    {
        public static int GetValueInt(this StatRecord statRecord)
        {
            return Mathf.RoundToInt(statRecord.GetValue());
        }
    }
}