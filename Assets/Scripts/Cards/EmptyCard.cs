using Photon.Pun;

namespace Cards
{
    public class EmptyCard : Card
    {
        [PunRPC]
        protected virtual void SetIsCaptured(bool value)
        {
            base.SetIsCaptured(value);
        }
        
        [PunRPC]
        protected virtual void SetCurrentUnitId(int value)
        {
            base.SetCurrentUnitId(value);
        }
    }
}