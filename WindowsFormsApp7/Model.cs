using System.Collections.Generic;

namespace WindowsFormsApp7
{
    public class Model
    {
        private static double T = 0;
        public static double Time { get { return T; } }
        private static List<Agent> agents = new List<Agent>();
        public static void Run()
        {
            T = 0;
            Agent activeAgent;
            do
            {

                double tmin = double.MaxValue;
                activeAgent = null;
                foreach (Agent a in agents)
                {
                    double ta = a.getNextEventTime();
                    if (ta < tmin)
                    {
                        tmin = ta;
                        activeAgent = a;
                    }
                }
                T = tmin;
                if (activeAgent != null) activeAgent.ProccessEvent();
            } while (!stopCondirion(T, activeAgent));

        }

        private static bool stopCondirion(double t, Agent activeAgent)
        {
            return (t > 100000) || (activeAgent == null);
        }
    }
}
