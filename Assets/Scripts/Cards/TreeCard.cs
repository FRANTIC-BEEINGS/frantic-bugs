namespace Cards
{
    public class TreeCard : Card, ICapturable
    {
        private int _visionRadius;
        public void Capture(ulong captorId)
        {
            if (IsCaptured) return; //todo: remove tmp fix and handle recapturing
            IsCaptured = true;
            CaptorId = captorId;
            //todo: change captor status on each card in radius
        }
    }
}