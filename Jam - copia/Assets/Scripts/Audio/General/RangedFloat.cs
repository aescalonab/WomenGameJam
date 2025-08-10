using System;

namespace Code.Audio.General
{
    [Serializable]
    public struct RangedFloat
    {
        public float minValue;
        public float maxValue;

        public void Init(float min, float max)
        {
            this.minValue = min;
            this.maxValue = max;
        }
    }
}
