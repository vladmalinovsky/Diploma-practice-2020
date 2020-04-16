using System;

namespace CombustionModeling
{
    class UnadaptedNet
    {
        public void TDMA(int N, double[] A, double[] C, double[] B, double[] F, double[] T)
        {
            double[] alpha = new double[N + 1];
            double[] beta = new double[N + 1];

            alpha[1] = B[0] / C[0];
            beta[1] = F[0] / C[0];
            double r;

            for (int i = 1; i < N; i++)
            {
                r = C[i] - A[i] * alpha[i];
                alpha[i + 1] = B[i] / r;
                beta[i + 1] = (A[i] * beta[i] + F[i]) / r;
            }

            T[N - 1] = beta[N];

            for (int i = N - 2; i >= 0; i--)
            {
                T[i] = alpha[i + 1] * T[i + 1] + beta[i + 1];
            }
        }

        public (double[], double[], int, double) NotAdoptedNetModeling()
        {
            int N = 2000;
            double startTemperature = 0.2;
            double endTemperature = 1.2;
            double startDensity = 1.0;
            double tau = 0.00001;
            double h = 1 / (double)N;
            double AA = Math.Pow(10.0, 10.0);
            double Le = 1;
            double Sigma = 18;
            double endTime = 0.2;
            double relative_eps = Math.Pow(10, -3);
            double absolute_eps = Math.Pow(10, -5);
            double c = 5;

            double[] T = new double[N];
            double[] R = new double[N];

            double[] T_iteration = new double[N];
            double[] R_iteration = new double[N];

            double[] T_previousIteration = new double[N];
            double[] R_previousIteration = new double[N];

            double[] A_t = new double[N];
            double[] C_t = new double[N];
            double[] B_t = new double[N];
            double[] F_t = new double[N];

            double[] A_r = new double[N];
            double[] C_r = new double[N];
            double[] B_r = new double[N];
            double[] F_r = new double[N];

            A_t[0] = 0;
            A_t[N - 1] = 1;
            A_r[0] = 0;
            A_r[N - 1] = 1;

            for (int i = 1; i < N - 1; i++)
            {
                A_t[i] = tau / (h * h);
                A_r[i] = Le * tau / (h * h);
            }

            B_t[0] = 0;
            B_t[N - 1] = 0;
            B_r[0] = 1;
            B_r[N - 1] = 0;

            for (int i = 1; i < N - 1; i++)
            {
                B_t[i] = tau / (h * h);
                B_r[i] = Le * tau / (h * h);
            }

            C_t[0] = 1;
            C_t[N - 1] = 1;
            C_r[0] = 1;
            C_r[N - 1] = 1;

            for (int i = 1; i < N - 1; i++)
            {
                C_t[i] = 1 + 2 * tau / (h * h);
                C_r[i] = 1 + 2 * Le * tau / (h * h);
            }

            F_t[0] = 1.2;
            F_t[N - 1] = 0;
            F_r[0] = 0;
            F_r[N - 1] = 0;

            for (int i = 0; i < N; i++)
            {
                T[i] = startTemperature;
                R[i] = startDensity;
            }

            double currentTime = 0;
            int totalNumberOfIterations = 0;

            while (currentTime < endTime)
            {
                currentTime += tau;

                CopyArray(T, T_iteration);
                CopyArray(R, R_iteration);

                bool continueIteration = true;
                int numberOfIterations = 0;

                F_t[0] = LeftBorderTemperature(c, startTemperature, endTemperature, currentTime);
                T_iteration[0] = LeftBorderTemperature(c, startTemperature, endTemperature, currentTime);

                do
                {
                    numberOfIterations++;
                    totalNumberOfIterations++;
                    CopyArray(T_iteration, T_previousIteration);
                    CopyArray(R_iteration, R_previousIteration);

                    for (int i = 1; i < N - 1; i++)
                    {
                        F_r[i] = R[i] - tau * AA * R_iteration[i] * Math.Exp(-Sigma / T_iteration[i]);
                        F_t[i] = T[i] + tau * AA * R_iteration[i] * Math.Exp(-Sigma / T_iteration[i]);
                    }

                    TDMA(N, A_r, C_r, B_r, F_r, R_iteration);
                    TDMA(N, A_t, C_t, B_t, F_t, T_iteration);
                    T_iteration[0] = LeftBorderTemperature(c, startTemperature, endTemperature, currentTime);

                    double eps_count = 0;

                    for (int i = 0; i < N; i++)
                    {
                        if (Math.Abs(T_iteration[i] - T_previousIteration[i]) < relative_eps * Math.Abs(T_previousIteration[i]) + absolute_eps)
                        {
                            eps_count++;
                        }
                    }

                    if (eps_count == T_iteration.Length)
                    {
                        continueIteration = false;
                        CopyArray(R_iteration, R);
                        CopyArray(T_iteration, T);
                    }
                }
                while (continueIteration);
            }

            return (T, R, N, h);
        }

        public double LeftBorderTemperature(double c, double startTemperature, double endTemperature, double time)
        {
            if(time <= 1 / c)
            {
                return startTemperature + c * time;
            }

            return endTemperature;
        }

        public void CopyArray(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                return;
            }

            for (int i = 0; i < a.Length; i++)
            {
                b[i] = a[i];
            }
        }
    }
}
