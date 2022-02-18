using System;

namespace Princess
{
    public class ExperimentalCaller/* : ICaller*/
    {
        public (int edgeNumber, float callForce) Straight(float rawDesire, bool[] punchcard, float[] rawcard, int[] solution)
        {
            (int edgeNumber, float callForce) callParams = (0, 0);

            float absCallForce = Math.Abs(rawDesire);
            int bestInSolution = 0;

            for(int i = 0; i < punchcard.Length; i++)
            {
                if(punchcard[i])
                {
                    if(solution[i] < 0 && rawcard[i] > -absCallForce && -solution[i] > bestInSolution)
                    {
                        bestInSolution = -solution[i];
                        callParams.edgeNumber = i;
                        callParams.callForce = -absCallForce;
                    }
                }
                else
                {
                    if(solution[i] > 0 && rawcard[i] < absCallForce && solution[i] > bestInSolution)
                    {
                        bestInSolution = solution[i];
                        callParams.edgeNumber = i;
                        callParams.callForce = absCallForce;
                    }
                }
            }

            return callParams;
        }

        public (int edgeNumber, float callForce) Inverted(float rawDesire, bool[] punchcard, float[] rawcard, int[] solution)
        {
            (int edgeNumber, float callForce) callParams = (0, 0);

            float absCallForce = Math.Abs(rawDesire);
            int bestInSolution = 0;

            for(int i = 0; i < punchcard.Length; i++)
            {
                if(punchcard[i])
                {
                    if(solution[i] < 0 && rawcard[i] < absCallForce && -solution[i] > bestInSolution)
                    {
                        bestInSolution = -solution[i];
                        callParams.edgeNumber = i;
                        callParams.callForce = absCallForce;
                    }
                }
                else
                {
                    if(solution[i] > 0 && rawcard[i] > -absCallForce && solution[i] > bestInSolution)
                    {
                        bestInSolution = solution[i];
                        callParams.edgeNumber = i;
                        callParams.callForce = -absCallForce;
                    }
                }
            }

            return callParams;
        }
    }
}
