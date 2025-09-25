using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WpfApp;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private string[] IfadeDonusum(string ifade)
    {
        List<String> characters = new List<string>();
        string buffer = "";

        foreach (char c in ifade)
        {
            if (char.IsDigit(c))
            {
                buffer += c;
            }
            else if (c.Equals(','))
            {
                buffer += '.';
            }
            else
            {
                if(buffer != "")
                {
                    characters.Add(buffer);
                    buffer = "";
                }
                characters.Add(c.ToString());
            }
        }
        if(buffer != "")
            characters.Add(buffer);

        return characters.ToArray();
    }

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var window = GetWindow(this);
        if (window != null) window.KeyDown += Key_Pressed;
    }
    
    private void Key_Pressed(object sender, KeyEventArgs e)
    {
        if (e.Key is >= Key.NumPad0 and <= Key.NumPad9)
        {
            ResultText.Text += e.Key - Key.NumPad0;
        }
        else if (e.Key is >= Key.D0 and <= Key.D9)
        {
            ResultText.Text += e.Key - Key.D0;
        }
        else
        {
            switch (e.Key)
            {
                case Key.OemComma:
                {
                    ResultText.Text += ",";
                    break;
                }
                case Key.Back:
                {
                    if(ResultText.Text != "")
                        ResultText.Text = ResultText.Text.Substring(0, ResultText.Text.Length - 1);
                    break;
                }
                case Key.Enter:
                {
                    ResultText.Text = GetResult(IfadeDonusum(ResultText.Text)).ToString("G").Replace('.', ',');
                    break;
                }
            }
        }
    }

    private void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        if(ResultText.Text != "")
            ResultText.Text = ResultText.Text.Substring(0, ResultText.Text.Length - 1);
    }
    
    private void Delete_All_Button_Click(object sender, RoutedEventArgs e)
    {
        ResultText.Text = "";
    }
    
    private bool equalClicked;
    
    private readonly List<String> operands = ["+", "-", "÷", "×"];
    private void Number_Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();

        if ((equalClicked && !operands.Any(item => IfadeDonusum(ResultText.Text).Contains(item))) 
            || (ResultText.Text.Equals("0") && !button.Content.Equals(",")))
        {
            ResultText.Text = button.Content.ToString() ?? string.Empty;
            equalClicked = false;
        }
        else 
        {
            ResultText.Text += button.Content.ToString();
        }
    }

    private void Equal_Button_Click(object sender, RoutedEventArgs e)
    {
        if(ResultText.Text != "")
        {
            string[] ifade = IfadeDonusum(ResultText.Text);

            ResultText.Text = ifade.Length > 2 ? GetResult(IfadeDonusum(ResultText.Text)).ToString("G").Replace('.', ',') : ifade[0];

            equalClicked = true;
        }
    }
    private void Operator_Button_Click(object sender, RoutedEventArgs e)
    {
        if(ResultText.Text != "")
        {
            Button button = sender as Button ?? throw new InvalidOperationException();

            string[] ifadeDizi = IfadeDonusum(ResultText.Text);

        
            if (operands.Any(item => ifadeDizi.Contains(item)) && !ifadeDizi.First().Equals("-"))
            {
                if (ifadeDizi.Length > 2)
                {
                    ResultText.Text = GetResult(ifadeDizi).ToString("G").Replace('.', ',') + button.Content;
                }
                else
                {
                    ResultText.Text = ifadeDizi[0] + button.Content;
                    if(equalClicked)
                        equalClicked = false;
                }
            }
            else if (ifadeDizi.First().Last().Equals('.'))
            {
                ResultText.Text = ifadeDizi.First().Substring(0, ifadeDizi.First().Length - 1) + button.Content;
                equalClicked = false;
            }
            else
            {
                ResultText.Text += button.Content.ToString();
            }
        }
    }

    private decimal GetResult(string[] ifade)
    {
        if (ifade.Length == 0) return 0;
        switch (ifade[1])
        {
            case "+":
            {
                return decimal.Parse(ifade[0], CultureInfo.InvariantCulture) 
                       + decimal.Parse(ifade[2], CultureInfo.InvariantCulture);
            }
            case "-":
            {
                return decimal.Parse(ifade[0], CultureInfo.InvariantCulture) 
                       - decimal.Parse(ifade[2], CultureInfo.InvariantCulture);
            }
            case "×":
            {
                return decimal.Parse(ifade[0], CultureInfo.InvariantCulture) 
                       * decimal.Parse(ifade[2], CultureInfo.InvariantCulture);
            }
            case "÷":
            {
                if(decimal.Parse(ifade[2], CultureInfo.InvariantCulture) != 0)
                    return decimal.Parse(ifade[0], CultureInfo.InvariantCulture) 
                           / decimal.Parse(ifade[2], CultureInfo.InvariantCulture);
                return 0;
            }
            default:
                return 0;
        }
    }
}