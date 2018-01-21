namespace ADTB
{
    public struct ADTBValues
    {
        public ADTBValues(float acX, float acY, float acZ, float tmp, float gyX, float gyY, float gyZ, float pitch, float roll) : this()
        {
            AcX = acX;
            AcY = acY;
            AcZ = acZ;
            Tmp = tmp;
            GyX = gyX;
            GyY = gyY;
            GyZ = gyZ;
            Pitch = pitch;
            Roll = roll;
        }

        public float Pitch { get; set; }
        public float Roll { get; set; }

        public float AcX { get; set; }
        public float AcY { get; set; }
        public float AcZ { get; set; }
        public float Tmp { get; set; }
        public float GyX { get; set; }
        public float GyY { get; set; }
        public float GyZ { get; set; }

        public override string ToString()
        {
            return string.Format("AcX : {0,5} | AcY : {1,5} | AcZ : {2,5} | GyX : {3,5} | GyY : {4,5} | GyZ : {5,5} | Tmp : {6,5}",
                AcX, AcY, AcY, GyX, GyY, GyZ, Tmp);
        }
    }
}
