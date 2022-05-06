using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TimesheetMobileApp2022k.Models;
using Xamarin.Forms;

namespace TimesheetMobileApp2022k
{
    public partial class EmployeePage : ContentPage
    {

        // Muuttujan alustaminen
        ObservableCollection<Employee> dataa = new ObservableCollection<Employee>();

        public EmployeePage()
        {
            InitializeComponent();

            // Kutsutaan alempana määriteltyä funktiota kun ohjelma käynnistyy
            LoadDataFromRestAPI();


              async void LoadDataFromRestAPI()
            {

                // Annetaan latausilmoitus
                emp_lataus.Text = "Ladataan työntekijöitä...";

                try
                {
                    // Tätä metodia tarvitaan kun ollaan debuggaustilassa ja käytetään paikallista back-endiä
                    HttpClientHandler GetInsecureHandler()
                    {
                        HttpClientHandler handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                        {
                            if (cert.Issuer.Equals("CN=localhost"))
                                return true;
                            return errors == System.Net.Security.SslPolicyErrors.None;
                        };
                        return handler;
                    }

#if DEBUG
                    HttpClientHandler insecureHandler = GetInsecureHandler(); // Lokaalia ajoa varten
                    HttpClient client = new HttpClient(insecureHandler);           //
#else
                    HttpClient client = new HttpClient();
#endif
                     client.BaseAddress = new Uri("https://10.0.2.2:7178/");
                    string json = await client.GetStringAsync("api/employees");

                    IEnumerable<Employee> employees = JsonConvert.DeserializeObject<Employee[]>(json);
                    // dataa -niminen observableCollection on alustettukin jo ylhäällä päätasolla että hakutoiminto,
                    // pääsee siihen käsiksi.
                    // asetetaan sen sisältö ensi kerran tässä pienellä kepulikonstilla:
                    ObservableCollection<Employee> dataa2 = new ObservableCollection<Employee>(employees);
                    dataa = dataa2;

                    // Asetetaan datat näkyviin xaml tiedostossa olevalle listalle
                    employeeList.ItemsSource = dataa;

                    // Tyhjennetään latausilmoitus label
                    emp_lataus.Text = "";

                }

                catch (Exception e)
                {
                    await DisplayAlert("Virhe", e.Message.ToString(), "SELVÄ!");
                
                }
            }
        }

        // Hakupalkin tapahtumankäsittelijä
        private void OnSearchBarButtonPressed(object sender, EventArgs args)
            {
                SearchBar searchBar = (SearchBar)sender;
                string searchText = searchBar.Text;

            if (searchText.Length > 0)
            {

                // Työntekijälistaukseen valitaan nyt vain ne joiden etu- tai sukunimeen sisältyy annettu hakutermi
                // "var dataa" on tiedoston päätasolla alustettu muuttuja, johon sijoitettiin alussa koko lista työntekijöistä.
                // Nyt sieltä valitaan vain hakuehdon täyttävät työntekijät xamlin employee listaukseen
                employeeList.ItemsSource = dataa.Where(x => x.LastName.ToLower().Contains(searchText.ToLower())
                || x.FirstName.ToLower().Contains(searchText.ToLower()));
            }

            }
        }
    }

