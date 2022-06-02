using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChromebookTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        CancellationTokenSource buttonCTS;

        async void Button_Clicked(object sender, EventArgs eventArgs)
        {
            Button button = sender as Button;
            if (buttonCTS != null) {
                Debug.WriteLine($"CANCEL");
                buttonCTS.Cancel();
                return;
            }
            try {
                Debug.WriteLine($"START");
                button.Text = "Calculating...";
                buttonCTS = new CancellationTokenSource();
                var result = await Calculate(100, 1000000, buttonCTS.Token);
                Debug.WriteLine($"SUCCESS\n{result}");
                await DisplayAlert("SUCCESS", result.ToString(), "OK");
            } catch (Exception ex) {
                Debug.WriteLine($"ERROR\n{ex}");
                await DisplayAlert("ERROR", ex.Message, "OK");
            } finally {
                button.Text = "Calculate";
                buttonCTS = null; ;
            }
        }

        Task<ulong> Calculate(uint m, uint n, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () => {
                ulong a = 0;
                while (0 != m--) {
                    cancellationToken.ThrowIfCancellationRequested();
                    Debug.WriteLine($"m={m+1}");
                    a += await Calculate(n, cancellationToken);
                }
                return a;
            });
        }

        Task<ulong> Calculate(uint n, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => {
                ulong a = 0;
                while (0 != n--) {
                    cancellationToken.ThrowIfCancellationRequested();
                    a += n;
                }
                return a;
            });
        }
    }
}
