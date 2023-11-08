using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        GreedyAlgorithm greedy = new GreedyAlgorithm();

        public Form1()
        {
            InitializeComponent();

            greedy.FillStartTable(dataGridView1);
        }

        private void addComponentButton_Click(object sender, EventArgs e)
        {
            greedy.AddComponent(dataGridView1);
        }

        private void AddProductButton_Click(object sender, EventArgs e)
        {
            greedy.AddProduct(dataGridView1);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            greedy.Save(dataGridView1);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            greedy.FillStartTable(dataGridView1);
        }

        private void ResetButton_Click_1(object sender, EventArgs e)
        {
            greedy.FillStartTable(dataGridView1);
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            label1.Text = greedy.Solve(tabPage2);
        }
    }

    public class GreedyAlgorithm
    {
        #region data
        // Задані дані
        Dictionary<string, double> availableComponents = new Dictionary<string, double>
        {
            { "Пігменти", 770 },
            { "Розчинники", 1900 },
            { "Загусники", 320 },
            { "Стабілізатори", 730 }
        };

        Dictionary<string, double> profitPerUnit = new Dictionary<string, double>
        {
            { "A", 3000 },
            { "B", 3200 },
            { "C", 2800 },
            { "D", 3100 }
        };

        Dictionary<string, Dictionary<string, double>> componentUsage = new Dictionary<string, Dictionary<string, double>>
        {
            { "Пігменти", new Dictionary<string, double> { { "A", 1 }, { "B", 0 }, { "C", 0 }, { "D", 0 } } },
            { "Розчинники", new Dictionary<string, double> { { "A", 0 }, { "B", 1 }, { "C",0 }, { "D", 0 } } },
            { "Загусники", new Dictionary<string, double> { { "A", 0 }, { "B", 0 }, { "C",1 }, { "D", 0 } } },
            { "Стабілізатори", new Dictionary<string, double> { { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 1 } } }
        };
        #endregion


        Dictionary<string, int> availableNumberOfBatches = new Dictionary<string, int>
        {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 },
            { "D", 0 }
        };

        Dictionary<string, int> totalBatch = new Dictionary<string, int>
        {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 },
            { "D", 0 }
        };

        public void SetDataGridViewSettings(DataGridView dataGrid)
        {
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            int columnCount = profitPerUnit.Count + 2;
            dataGrid.ColumnCount = columnCount;

            foreach (DataGridViewColumn column in dataGrid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGrid.AutoGenerateColumns = true;
        }

        public void FillStartTable(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();
            dataGrid.Refresh();

            SetDataGridViewSettings(dataGrid);
            int columnCount = profitPerUnit.Count + 2;

            dataGrid.Columns[0].Name = "Вид сировини / Норми витрат сировини (л) на одну партію виробів";

            for (int i = 0; i < profitPerUnit.Count; i++)
            {
                dataGrid.Columns[i + 1].Name = profitPerUnit.ElementAt(i).Key;
            }

            dataGrid.Columns[columnCount - 1].Name = "Загальна кількість сировини (л)";

            for (int i = 0; i < availableComponents.Count; i++)
            {
                string[] row = new string[columnCount];

                row[0] = availableComponents.ElementAt(i).Key;

                Dictionary<string, double> e = componentUsage.ElementAt(i).Value;

                for (int j = 0; j < e.Count; j++)
                {
                    row[j + 1] = e.ElementAt(j).Value.ToString();
                }

                row[columnCount - 1] = availableComponents.ElementAt(i).Value.ToString();


                dataGrid.Rows.Add(row);
            }

            string[] lastRow = new string[columnCount];

            lastRow[0] = "Прибуток від реалізації партії виробів одного виду (грн.)";

            for (int j = 0; j < profitPerUnit.Count; j++)
            {
                lastRow[j + 1] = profitPerUnit.ElementAt(j).Value.ToString();
            }

            lastRow[columnCount - 1] = "";

            dataGrid.Rows.Add(lastRow);
        }

        public void AddComponent(DataGridView dataGrid)
        {
            Save(dataGrid);

            string name = "Component" + (availableComponents.Count + 1).ToString();
            availableComponents.Add(name, 0d);

            Dictionary<string, double> newCmponentUsage = new Dictionary<string, double>();
            foreach (var item in profitPerUnit)
            {
                newCmponentUsage.Add(item.Key, 1d);
            }
            componentUsage.Add(name, newCmponentUsage);

            FillStartTable(dataGrid);
        }

        public void AddProduct(DataGridView dataGrid)
        {
            Save(dataGrid);

            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string name = alphabet[profitPerUnit.Count + 1].ToString();

            profitPerUnit.Add(name, 0d);
            availableNumberOfBatches.Add(name, 0);
            totalBatch.Add(name, 0);

            for (int i = 0; i < componentUsage.Count; i++)
            {
                Dictionary<string, double> d = componentUsage.ElementAt(i).Value;
                d.Add(name, 1d);
            }

            FillStartTable(dataGrid);
        }

        public void Save(DataGridView dataGrid)
        {
            //первый столбик
            Dictionary<string, double> newAvailableComponents = new Dictionary<string, double>();
            Dictionary<string, Dictionary<string, double>> newComponentUsage = new Dictionary<string, Dictionary<string, double>>();

            for (int i = 0; i < dataGrid.Rows.Count - 1; i++)
            {
                double value = availableComponents.ElementAt(i).Value;
                newAvailableComponents.Add(dataGrid.Rows[i].Cells[0].Value.ToString(), value);

                Dictionary<string, double> dValue = componentUsage.ElementAt(i).Value;
                newComponentUsage.Add(dataGrid.Rows[i].Cells[0].Value.ToString(), dValue);
            }
            availableComponents = newAvailableComponents;
            componentUsage = newComponentUsage;

            //профит за партию
            Dictionary<string, double> newProfitPerUnit = new Dictionary<string, double>();
            for (int i = 0; i < profitPerUnit.Count; i++)
            {
                double v = Convert.ToDouble(dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[i + 1].Value);
                newProfitPerUnit.Add(dataGrid.Columns[i + 1].Name, v);
            }
            profitPerUnit = newProfitPerUnit;


            //последний столбик / доступные ресурсы
            Dictionary<string, double> newAvailableComponents2 = new Dictionary<string, double>();

            for (int i = 0; i < dataGrid.Rows.Count - 1; i++)
            {
                double value = Convert.ToDouble(dataGrid.Rows[i].Cells[dataGrid.ColumnCount - 1].Value.ToString());
                newAvailableComponents2.Add(availableComponents.ElementAt(i).Key, value);
            }
            availableComponents = newAvailableComponents2;

            //средние клетки 
            Dictionary<string, Dictionary<string, double>> newComponentUsage2 = new Dictionary<string, Dictionary<string, double>>();

            for (int i = 0; i < componentUsage.Count; i++)
            {
                var qwe = componentUsage.ElementAt(i).Value;
                Dictionary<string, double> v = new Dictionary<string, double>();

                for (int j = 0; j < qwe.Count; j++)
                {
                    v.Add(qwe.ElementAt(j).Key, Convert.ToDouble(dataGrid.Rows[i].Cells[j + 1].Value.ToString()));
                }

                newComponentUsage2.Add(availableComponents.ElementAt(i).Key, v);
            }
            componentUsage = newComponentUsage2;
        }

        private void ResetListValue()
        {
            Dictionary<string, int> newTotalBatch = new Dictionary<string, int>();

            foreach (var item in totalBatch)
            {
                newTotalBatch.Add(item.Key, 0);
            }
            totalBatch = newTotalBatch;

            Dictionary<string, int> newAvailableNumberOfBatches = new Dictionary<string, int>();

            foreach (var item in availableNumberOfBatches)
            {
                newAvailableNumberOfBatches.Add(item.Key, 0);
            }
            availableNumberOfBatches = newAvailableNumberOfBatches;
        }

        public string Solve(TabPage tabPage)
        {
            ResetListValue();

            int step = 0;
            tabPage.Controls.Clear();

            while (true)
            {
                bool canProduce = false;
                var productsWithRatios = new Dictionary<string, double>();
                foreach (var product in profitPerUnit.Keys)
                {
                    int minRatio = int.MaxValue;
                    double litersPerBatch = 0;

                    foreach (var component in availableComponents.Keys)
                    {
                        double componentUsageForProduct = componentUsage[component][product];
                        litersPerBatch += componentUsageForProduct;
                        if (componentUsageForProduct > 0)
                        {
                            int availableAmount = (int)(availableComponents[component] / componentUsageForProduct);
                            if (availableAmount < minRatio)
                            {
                                minRatio = availableAmount;
                                availableNumberOfBatches[product] = minRatio;
                            }
                        }
                    }

                    double profitPerUnitForProduct = profitPerUnit[product];

                    double ratio = profitPerUnitForProduct / litersPerBatch;
                    productsWithRatios[product] = ratio;
                }

                int count = 0;
                string bestProduct = "";

                foreach (var item in productsWithRatios.OrderByDescending(pair => pair.Value))
                {
                    bestProduct = item.Key;
                    count = availableNumberOfBatches[bestProduct];
                    if (count > 0)
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(bestProduct) && count > 0)
                {
                    canProduce = true;

                    foreach (var component in availableComponents.Keys.ToList())
                    {
                        double componentUsageForProduct = componentUsage[component][bestProduct];
                        availableComponents[component] -= componentUsageForProduct * count;
                    }

                    totalBatch[bestProduct] += count;
                }

                if (!canProduce)
                {
                    break;
                }

                CreateStepFromDataGrid(tabPage, step);

                step++;
            }

            string res = "";
            foreach (var item in totalBatch)
            {
                res += $"Кількість фарби {item.Key}: { item.Value} \n";
            }

            return res;
        }

        public void CreateStepFromDataGrid(TabPage tabPage, int step)
        {
            //дані
            int dataGridWidth = 700;
            int dataGridHeight = 400;

            int dataGridX = 6;
            int dataGridY = 6 * step + dataGridHeight * step;


            int stepLabelWidth = 700;
            int stepLabelHeight = 400;
            int stepLabelX = dataGridWidth + 30;
            int stepLabelY = 6 * step + dataGridHeight * step;

            //написи
            Label stepLabel = new Label();
            tabPage.Controls.Add(stepLabel);
            stepLabel.Text = $"Крок {step + 1}";
            stepLabel.Location = new Point(stepLabelX, stepLabelY);

            //таблиці
            DataGridView grid = new DataGridView();
            tabPage.Controls.Add(grid);

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            int columnCount = profitPerUnit.Count + 2;
            grid.ColumnCount = columnCount;
            grid.Size = new Size(dataGridWidth, dataGridHeight);
            grid.Location = new Point(dataGridX, dataGridY);

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            grid.AutoGenerateColumns = true;

            FillStartTable(grid);

            //результати

            Label resLabel = new Label();
            tabPage.Controls.Add(resLabel);
            resLabel.Location = new Point(labelX, labelY);

            string res = "";
            foreach (var item in totalBatch)
            {
                res += $"Кількість фарби {item.Key}: { item.Value} \n";
            }

            resLabel.Text = res;
        }
    }
}
