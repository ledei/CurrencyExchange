using System.Windows;
using System.Windows.Input;

//This library is used for Regular Expression
using System.Text.RegularExpressions;

//This library is used for DataTable
using System.Data;
using System.Threading.Tasks;
using CurrencyExchange;
using System.Net.Http;
using System;
using Newtonsoft.Json;

namespace CurrencyConverter_Static
{
    public partial class MainWindow : Window
    {
        Root val = new Root();

        public MainWindow()
        {
            InitializeComponent();

            //ClearControls method is used to clear all control values
            ClearControls();
            GetValue();
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=");
            BindCurrency();
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage res = await client.GetAsync(url);
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var resString = await res.Content.ReadAsStringAsync();
                        var resObject = JsonConvert.DeserializeObject<Root>(resString);

                        return resObject;
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        #region Bind Currency From and To Combobox
        private void BindCurrency()
        {
            // Create a DataTable Object
            DataTable dtCurrency = new DataTable();

            // Add the text column in the DataTable
            dtCurrency.Columns.Add("Text");

            // Add the value column in the DataTable
            dtCurrency.Columns.Add("Value");

            // Populate the DataTable with currency data
            PopulateCurrencyData(dtCurrency);

            // Bind the data to the From Currency combobox
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            // Bind the data to the To Currency combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void PopulateCurrencyData(DataTable dtCurrency)
        {
            if (val?.rates == null)
                return;

            // Use reflection to get all properties of the Rate class
            var properties = typeof(Rate).GetProperties();

            // Iterate over each property and add it to the DataTable
            foreach (var prop in properties)
            {
                var currencyCode = prop.Name;
                var currencyValue = prop.GetValue(val.rates);

                // Ensure the currencyValue is a double and not zero
                if (currencyValue != null)
                {
                    string currencyValueString = currencyValue.ToString();
                    double value;
                    if (double.TryParse(currencyValueString, out value) && value != 0)
                    {
                        dtCurrency.Rows.Add(currencyCode, value);
                    }
                }
            }
        }

        #endregion

        #region Button Click Event

        //Convert the button click event
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create the variable as ConvertedValue with double datatype to store currency converted value
            double ConvertedValue;

            //Check if the amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After clicking on messagebox OK set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //Check if From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String To Double.
                //Textbox text have string and ConvertedValue is double Datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency converter is From Currency value multiply(*) 
                //With the amount textbox value and then that total divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        //Clear Button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method is used to clear all controls value
            ClearControls();
        }
        #endregion

        #region Extra Events

        //ClearControls method is used to clear all controls value
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
            // Add Library using System.Text.RegularExpressions;
            Regex regex = new Regex("^[0-9]+");
            e.Handled = !regex.IsMatch(e.Text);

            /* Regex regex = new Regex("^[0-9]+$");
             e.Handled = !regex.IsMatch(e.Text);*/
        }
        #endregion
    }
}