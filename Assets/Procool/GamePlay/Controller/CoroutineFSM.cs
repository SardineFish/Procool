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
                StopAllCoroutines();
            currentState = StartCoroutine(state);
        }
    }
}