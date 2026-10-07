using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _01.Scripts.Util
{
    // 매번 하기 귀찮아서 만들엇음
    public static class UTaskUtil
    {
        public static void Kill(ref CancellationTokenSource cts)
        {
            if (cts == null) return;
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        
        public static async UniTask TryUniTask(UniTask task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }
}