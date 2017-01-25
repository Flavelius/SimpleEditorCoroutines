using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SimpleEditorCoroutines.Test
{
    public class EditorCoroutineTest : EditorWindow
    {

        [MenuItem("Window/EditorCoroutine Test")]
        static void Open()
        {
            GetWindow<EditorCoroutineTest>("CoroutineTest");
        }

        List<PrimeTest> Tests = new List<PrimeTest>()
        {
            new PrimeTest(5000)
        };

        void OnGUI()
        {
            for (var i = 0; i < Tests.Count; i++)
            {
                Tests[i].DrawGUI();
            }
            if (GUILayout.Button("+"))
            {
                Tests.Add(new PrimeTest(10000));
            }
        }

        class PrimeTest
        {
            public PrimeTest(int maxPrimes)
            {
                numPrimes = maxPrimes;
            }

            int numPrimes;
            EditorCoroutine runningRoutine;

            public void DrawGUI()
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Calculate prime numbers");
                numPrimes = EditorGUILayout.IntField("Stop After", numPrimes);
                numPrimes = Mathf.Clamp(numPrimes, 100, 100000);
                if (runningRoutine == null || runningRoutine.State == EditorCoroutineState.Canceled || runningRoutine.State == EditorCoroutineState.Done)
                {
                    if (GUILayout.Button("Start"))
                    {
                        runningRoutine = EditorCoroutine.Start(FindPrimeNumber(numPrimes));
                    }
                }
                else
                {
                    if (runningRoutine != null && GUILayout.Button("Cancel"))
                    {
                        runningRoutine.Cancel();
                    }
                }
                if (runningRoutine != null)
                {
                    var prevColor = GUI.backgroundColor;
                    switch (runningRoutine.State)
                    {
                            case EditorCoroutineState.Running:
                            GUI.backgroundColor = Color.yellow;
                            break;
                        case EditorCoroutineState.Canceled:
                            GUI.backgroundColor = Color.red;
                            break;
                            case EditorCoroutineState.Done:
                            GUI.backgroundColor = Color.green;
                            break;
                    }
                    EditorGUILayout.LabelField(runningRoutine.State.ToString(), EditorStyles.helpBox);
                    GUI.backgroundColor = prevColor;
                }
                EditorGUILayout.EndVertical();
            }
        }

        void Update()
        {
            Repaint();
        }   

        //borrowed from StackExchange
        static IEnumerator FindPrimeNumber(int n)
        {
            var count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                var prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
                if (count % 1000 == 0)
                {
                    yield return null;
                }
            }
        }
    }
}
