using N2.Engine;

namespace N2.Web.Messaging
{
    public class ReceiverAttribute : ServiceAttribute
    {
        public ReceiverAttribute()
            : base (typeof(IReceiver))
        {
        }
    }
}
