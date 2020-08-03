using System.Collections;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public class CoroutineFSM : MonoBehaviour
    {
        private Coroutine currentState;

        public void ChangeState(IEnumerator state)
        {
            if(currentState != null)
                StopCoroutine(currentState);
            StartCoroutine(state);
        }
    }
}