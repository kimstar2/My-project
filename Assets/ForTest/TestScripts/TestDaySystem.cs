using System.Threading;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.TimeService;
using UnityEngine;

namespace ForTest.TestScripts
{
    public class TestDaySystem : MonoBehaviour
    {
        private CancellationTokenSource _cts;
        private void Start()
        {
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            ServiceLocator.GetService<ITimeService>().ActionTimer(5f,token,
                null,HandleTimeEnd);
        }

        private void HandleTimeEnd()
        {
            Debug.Log("밤");
        }
    }
}
