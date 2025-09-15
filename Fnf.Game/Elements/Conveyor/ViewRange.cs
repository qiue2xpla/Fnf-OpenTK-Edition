using Fnf.Framework;
using System;

namespace Fnf.Game
{
    internal class ViewRange
    {/*
        public int UpperIndex;
        public int LowerIndex;

        private ConveyorBase _conveyor;
        private Chart _chart => _conveyor.chart;

        private float _upperTimeOffset;
        private float _lowerTimeOffset;

        public ViewRange(ConveyorBase conveyor)
        {
            _conveyor = conveyor;
        }

        public void Update()
        {
            CalculateTimeOffsets();

            float musicPosition = (float)Music.Position;

            // Loopback is not supported
            for (int i = UpperIndex; i < _chart.notes.Length; i++)
            {
                float delay = musicPosition - _chart.notes[i].delay - _chart.notes[i].length;

                if (delay > _upperTimeOffset) UpperIndex++;
                else break;
            }

            for (int i = LowerIndex; i < _chart.notes.Length; i++)
            {
                float delay = musicPosition - _chart.notes[i].delay;

                if (delay < _lowerTimeOffset) break;

                LowerIndex++;
            }

            if (UpperIndex >= _chart.notes.Length) LowerIndex = _chart.notes.Length - 1;
            if (LowerIndex >= _chart.notes.Length) LowerIndex = _chart.notes.Length - 1;
        }

        void CalculateTimeOffsets()
        {
            float _topPosition = float.NegativeInfinity;
            float _bottomPosition = float.PositiveInfinity;

            for (int column = 0; column < _conveyor.columns.Length; column++)
            {
                float angle = -_conveyor.columns[column].animator.globalRotation / 180 * (float)Math.PI;
                float cos = (float)Math.Cos(angle);
                
                // Top point
                float top = Window.GridSize.height - _conveyor.columns[column].animator.globalPosition.y;
                top /= cos;

                // Bottom point
                float bottom = -_conveyor.columns[column].animator.globalPosition.y;
                bottom /= cos;

                _topPosition = Math.Max(top, _topPosition);
                _bottomPosition = Math.Min(bottom, _bottomPosition);
            }

            _upperTimeOffset = _topPosition / ConveyorBase.DistanceInSecond;
            _lowerTimeOffset = _bottomPosition / ConveyorBase.DistanceInSecond;

            //will be moved to note skin
            float noteWidth = 220 * _conveyor.globalScale.y;
            float offsetInSeconds = noteWidth / ConveyorBase.DistanceInSecond;

            // Add offsets so the note doesnt just appear out of thin air
            _upperTimeOffset += offsetInSeconds;
            _lowerTimeOffset -= offsetInSeconds;
        }*/
    }
}