using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace SimpleEditorCoroutines
{

    public enum EditorCoroutineState { Running, Done, Canceled }

    public class EditorCoroutine
    {
        readonly IEnumerator routineReference;
        readonly Action<EditorCoroutine> cancel;

        /// <summary>
        /// Use this to start the Editor coroutine.
        /// </summary>
        /// <returns>Returns an object that can be used to check the status of or cancel the routine</returns>
        public static EditorCoroutine Start(IEnumerator routine)
        {
            return EditorCoroutines.StartRoutine(routine);
        }

        EditorCoroutine(IEnumerator routine, Action<EditorCoroutine> cancelAction)
        {
            routineReference = routine;
            cancel = cancelAction;
        }

        public EditorCoroutineState State { get; private set; }

        public void Cancel()
        {
            cancel(this);
        }

        static class EditorCoroutines
        {
            static readonly List<EditorCoroutine> activeRoutines = new List<EditorCoroutine>();

            static void UpdateActiveRoutines()
            {
                for (var i = activeRoutines.Count; i-- > 0;)
                {
                    if (!activeRoutines[i].routineReference.MoveNext())
                    {
                        activeRoutines[i].State = EditorCoroutineState.Done;
                        activeRoutines.RemoveAt(i);
                    }
                }
                if (activeRoutines.Count > 0)
                {
                    EditorApplication.delayCall += UpdateActiveRoutines;
                }
            }

            public static EditorCoroutine StartRoutine(IEnumerator routine)
            {
                var ec = new EditorCoroutine(routine, CancelRoutine);
                if (activeRoutines.Count == 0) EditorApplication.delayCall += UpdateActiveRoutines;
                activeRoutines.Add(ec);
                ec.State = EditorCoroutineState.Running;
                return ec;
            }

            static void CancelRoutine(EditorCoroutine routine)
            {
                routine.State = EditorCoroutineState.Canceled;
                activeRoutines.Remove(routine);
            }
        }

}
}
