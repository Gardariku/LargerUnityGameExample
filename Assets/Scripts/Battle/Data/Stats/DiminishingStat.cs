using System;
using CleverCrow.Fluid.StatsSystem;
using UnityEngine.Events;

namespace Battle.Data.Stats
{
    public class DiminishingStat
    {
        public StatRecord MainStat { get; }
        public int CurrentValue { get; set; }
        private int _previousMaxValue;

        public UnityAction<StatRecord> StatDiminished;

        public DiminishingStat(StatRecord statRecord)
        {
            MainStat = statRecord;
            CurrentValue = (int) statRecord.GetValue();
            _previousMaxValue = CurrentValue;
            StatDiminished += OnStatDiminished;
        }

        public void OnStatDiminished(StatRecord statRecord)
        {
            int currentMaxValue = (int) statRecord.GetValue();
            float modifier = Math.Abs(currentMaxValue / _previousMaxValue);
            CurrentValue = (int) (CurrentValue * modifier);
        }
    }
}