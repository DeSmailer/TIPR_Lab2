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
        public Form1()
        {
            InitializeComponent();

            GreedyAlgorithm greedy = new GreedyAlgorithm();
            label1.Text = greedy.Solve();
        }
    }

    public class GreedyAlgorithm
    {
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
            { "Пігменти", new Dictionary<string, double> { { "A", 4 }, { "B", 3 }, { "C", 5 }, { "D", 3.5 } } },
            { "Розчинники", new Dictionary<string, double> { { "A", 8 }, { "B", 9 }, { "C", 8 }, { "D", 7.5 } } },
            { "Загусники", new Dictionary<string, double> { { "A", 1 }, { "B", 1.5 }, { "C", 1.3 }, { "D", 0.8 } } },
            { "Стабілізатори", new Dictionary<string, double> { { "A", 2 }, { "B", 3 }, { "C", 2.7 }, { "D", 2 } } }
        };

        // Ініціалізація змінних для кількості фарб
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

        public string Solve()
        {
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
                        else
                        {
                            minRatio = 0;
                            break;
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
            }

            string res = "";

            foreach (var item in totalBatch)
            {
                res += $"Кількість фарби {item.Key}: { item.Value} \n";
            }

            return res;
        }
    }
}
