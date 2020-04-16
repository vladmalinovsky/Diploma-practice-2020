using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CombustionModeling
{
    public partial class Form1 : Form
    {
        Chart chart1;

        public Form1()
        {
            CustomInitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Add("Temperature");
            chart1.Series.Add("Density");
            chart1.Series["Temperature"].ChartType = SeriesChartType.Spline;
            chart1.Series["Density"].ChartType = SeriesChartType.Spline;

            UnadaptedNet unadaptedNet = new UnadaptedNet();
            (double[], double[], int, double) result = unadaptedNet.NotAdoptedNetModeling();

            double[] T = result.Item1;
            double[] R = result.Item2;
            int N = result.Item3;
            double h = result.Item4;
            double x = 0;

            for (int i = 0; i < N; i++)
            {
                double T_y = T[i];
                double R_y = R[i];

                chart1.Series["Temperature"].Points.AddXY(x, T_y);
                chart1.Series["Density"].Points.AddXY(x, R_y);
                x += h;
            }

            Controls.Add(chart1);
        }

        private void CustomInitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.chart1 = new Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();

            ChartArea chartArea1 = new ChartArea();
            chartArea1.AxisX.Title = "X";
            chartArea1.AxisY.Title = "Y";
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);

            Legend legend1 = new Legend();
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1000, 500);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";

            this.Load += new EventHandler(this.Form1_Load);
            this.ClientSize = new System.Drawing.Size(1200, 600);
        }
    }
}
