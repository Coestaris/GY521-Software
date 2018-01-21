using System.Linq;

namespace ADTB
{
    public class ADTBSmoothResultStack
    {
        private ADTBValues[] values;
        private int smoothLevel;
        private ulong counter;
        private bool fill;

        public ADTBSmoothResultStack(int smoothLevel = 2)
        {
            this.smoothLevel = smoothLevel;
            values = new ADTBValues[smoothLevel];
        }

        public void Push(ADTBValues value)
        {
            if(counter < (ulong)smoothLevel)
                if(!fill)
                {
                    fill = true;
                    for (int i = 0; i < smoothLevel; i++)
                        values[i] = value;
                    return;
                }

            if (counter >= ulong.MaxValue)
                counter = 0;

            values[counter++ % (ulong)smoothLevel] = value;
        }

        public ADTBValues Current()
        {
            return new ADTBValues(
                values.Average(p => p.AcX),
                values.Average(p => p.AcY),
                values.Average(p => p.AcZ),
                values.Average(p => p.Tmp),
                values.Average(p => p.GyX),
                values.Average(p => p.GyY),
                values.Average(p => p.GyZ),
                values.Average(p => p.Pitch),
                values.Average(p => p.Roll));
        }
    }
}
